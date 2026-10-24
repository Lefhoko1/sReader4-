using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SReader.Game.Trek;
using SReader.Domains.Assignments.Models;
namespace SReader.Game.Classroom
{
    public sealed class ClassroomBoard : MonoBehaviour
    {
        public TMP_Text assignmentTitle, completion, notice, pagination;
        public TMP_Text[] wordLabels;
        public Button[] wordButtons;
        public TextAsset previewContent;
        public ClassroomCameraRig rig;
        public event Action<int> WordSelected;
        public event Action FinishRequested;
        public event Action PageChanged;
        public int PageStart => page * wordLabels.Length;
        TrailData trail; TrekSession session; int page;
        static readonly Color Chalk = new Color(.95f,.94f,.87f), Gold = new Color(.84f,.68f,.35f), Muted = new Color(.59f,.66f,.59f);
        public TrailData Trail => trail;
        void Awake()
        {
            var more = pagination.GetComponent<Button>(); if (more) more.onClick.AddListener(NextPage);
            var finish = completion.GetComponent<Button>(); if (finish) finish.onClick.AddListener(() => { if(session != null && session.CanComplete) FinishRequested?.Invoke(); });
            for (int i=0;i<wordButtons.Length;i++) { int row=i; wordButtons[i].onClick.AddListener(()=>Select(row)); }
        }
        void Start()
        {
            if (trail == null && previewContent) Bind(TrailBuilder.Build("classroom-practice","Meadow at Dawn",AssignmentContentCodec.FromJson(previewContent.text)),null);
        }
        public void Bind(TrailData data, TrekSession state) { trail=data; session=state; page=0; Refresh(); }
        public void ShowWord(int slot) { if(trail==null || trail.Gates.Count==0)return; page=Mathf.Clamp(page,0,(trail.Gates.Count-1)/wordLabels.Length); Refresh(); }
        public void NextPage() { if (trail==null) return; page=(page+1)%Mathf.Max(1,Mathf.CeilToInt(trail.Gates.Count/(float)wordLabels.Length)); Refresh(); PageChanged?.Invoke(); }
        public void Refresh()
        {
            if (trail==null) return;
            assignmentTitle.text=trail.Title;
            int solved=session?.SolvedGates??0;
            completion.text=$"Words completed  {solved} / {trail.Gates.Count}";
            notice.text=session==null?"Tap a word to begin":session.CanComplete?"Tap Words completed to finish":"Restore words in passage order";
            for (int row=0;row<wordLabels.Length;row++)
            {
                int i=page*wordLabels.Length+row; bool show=i<trail.Gates.Count; wordButtons[row].gameObject.SetActive(show);
                if (!show) continue;
                var gate=trail.Gates[i]; bool won=IsSolved(gate.State);
                wordLabels[row].text=(won?"+  ":"")+gate.Word+(gate.State==GateState.Deferred?"  (Later)":"");
                wordLabels[row].color=won?Gold:Available(i)?Chalk:Muted;
            }
            pagination.text=trail.Gates.Count>wordLabels.Length?$"More words   {page+1}/{Mathf.CeilToInt(trail.Gates.Count/(float)wordLabels.Length)}":"The Great Library of Knowledge";
        }
        public bool Available(int index) => trail!=null && index>=0 && index<trail.Gates.Count && (session==null?index==0:session.ActiveGate!=null?session.ActiveGate==trail.Gates[index]:session.PeekNextGate()==trail.Gates[index]);
        public void SelectWord(int index)
        {
            if (trail==null || index<0 || index>=trail.Gates.Count) return;
            if (rig.currentView!=1) { rig.GoTo(1);return; }
            if (Available(index)) WordSelected?.Invoke(index);
            else notice.text=IsSolved(trail.Gates[index].State)?"This word has been restored":"Read the next word first, or choose Later";
        }
        void Select(int row) => SelectWord(page*wordLabels.Length+row);
        public static bool IsSolved(GateState state) => state==GateState.SolvedGold||state==GateState.SolvedSilver||state==GateState.SolvedBronze;
    }
}
