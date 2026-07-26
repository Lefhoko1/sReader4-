using System;
using System.Collections.Generic;
using UnityEngine;

namespace SReader.Game.Trek
{
    /// <summary>
    /// Persists the trek-specific half of the resume point (deferred queue, hint
    /// usage, elapsed seconds) on the device. The solved-gates half stays in the
    /// EXISTING GameProgress store — this is a purely ADDITIVE side-car
    /// (PlayerPrefs JSON), so no existing schema changes (spec guardrail).
    /// </summary>
    internal static class TrekCheckpointStore
    {
        const string KeyPrefix = "sreader.trek.cp.";

        [Serializable]
        class Dto
        {
            public string assignmentId;
            public int lastSolvedGateIndex = -1;
            public List<string> deferredGateIds = new List<string>();
            public List<string> hintGateIds = new List<string>();
            public List<int> hintLevels = new List<int>();
            public float secondsElapsed;
            public int pointsSoFar;
        }

        static string Key(string userId, string assignmentId)
            => KeyPrefix + (string.IsNullOrEmpty(userId) ? "local" : userId) + "." + assignmentId;

        public static void Save(string userId, TrekCheckpoint cp)
        {
            if (cp == null || string.IsNullOrEmpty(cp.AssignmentId)) return;
            var dto = new Dto
            {
                assignmentId = cp.AssignmentId,
                lastSolvedGateIndex = cp.LastSolvedGateIndex,
                deferredGateIds = new List<string>(cp.DeferredGateIds),
                secondsElapsed = cp.SecondsElapsed,
                pointsSoFar = cp.PointsSoFar
            };
            foreach (var kv in cp.HintsUsedByGate)
            {
                dto.hintGateIds.Add(kv.Key);
                dto.hintLevels.Add(kv.Value);
            }
            PlayerPrefs.SetString(Key(userId, cp.AssignmentId), JsonUtility.ToJson(dto));
            PlayerPrefs.Save();
        }

        public static TrekCheckpoint Load(string userId, string assignmentId)
        {
            var json = PlayerPrefs.GetString(Key(userId, assignmentId), "");
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                var dto = JsonUtility.FromJson<Dto>(json);
                if (dto == null) return null;
                var cp = new TrekCheckpoint
                {
                    AssignmentId = dto.assignmentId,
                    LastSolvedGateIndex = dto.lastSolvedGateIndex,
                    DeferredGateIds = dto.deferredGateIds ?? new List<string>(),
                    SecondsElapsed = dto.secondsElapsed,
                    PointsSoFar = dto.pointsSoFar
                };
                for (int i = 0; i < dto.hintGateIds.Count && i < dto.hintLevels.Count; i++)
                    cp.HintsUsedByGate[dto.hintGateIds[i]] = dto.hintLevels[i];
                return cp;
            }
            catch { return null; }
        }

        public static void Clear(string userId, string assignmentId)
        {
            PlayerPrefs.DeleteKey(Key(userId, assignmentId));
            PlayerPrefs.Save();
        }
    }
}
