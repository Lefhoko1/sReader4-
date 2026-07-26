using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A grade/level an academy offers — e.g. "PSLE" (Primary), "JC" or
    /// "BGCSE" (Secondary), or a university program like "BSc Computer
    /// Science". Primary/Secondary grades are certificate-based and carry no
    /// courses; only University grades register courses (see AcademyCourse).
    /// </summary>
    public class AcademyGrade : Entity
    {
        public string AcademyId { get; set; }
        public EducationStage Stage { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// University programs register "modules"; Primary/Secondary grades
        /// register "subjects". Both are stored as AcademyCourse rows.
        /// </summary>
        public bool UsesModules => Stage == EducationStage.University;
    }
}
