namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// Static catalogue of Botswana's school structure used to drive the
    /// grade/subject pickers:
    ///   Primary  → Standard 1–7        (certificate: PSLE)
    ///   Secondary→ Form 1–3 (JC), Form 4–5 (BGCSE)
    ///   University programs register modules (free text) instead of subjects.
    /// Subjects below span PSLE, JC and BGCSE offerings.
    /// </summary>
    public static class BotswanaCurriculum
    {
        public static readonly string[] PrimaryLevels =
        {
            "Standard 1", "Standard 2", "Standard 3", "Standard 4",
            "Standard 5", "Standard 6", "Standard 7"
        };

        public static readonly string[] SecondaryLevels =
        {
            "Form 1", "Form 2", "Form 3", "Form 4", "Form 5"
        };

        /// <summary>Subjects offered across PSLE / JC / BGCSE (alphabetical-ish, deduped).</summary>
        public static readonly string[] Subjects =
        {
            "English",
            "Setswana",
            "Mathematics",
            "Additional Mathematics",
            "Statistics",
            "Science",
            "Integrated Science",
            "Combined Science",
            "Biology",
            "Physics",
            "Chemistry",
            "Agriculture",
            "Geography",
            "History",
            "Social Studies",
            "Development Studies (DVS)",
            "Religious Education",
            "Moral Education",
            "Commerce",
            "Accounting",
            "Business Studies",
            "Economics",
            "Design and Technology (D&T)",
            "Art and Design",
            "Music",
            "Home Economics (HE)",
            "Food and Nutrition",
            "Fashion and Fabrics",
            "Physical Education (PE)",
            "Creative and Performing Arts (CAPA)",
            "Computer Studies",
            "Information & Communication Technology (ICT)",
            "Office Management",
            "Literature in English",
            "Setswana Literature",
            "French"
        };

        /// <summary>The certificate a level belongs to (for display): PSLE / JC / BGCSE.</summary>
        public static string CertificateFor(EducationStage stage, string levelTitle)
        {
            if (stage == EducationStage.Primary) return "PSLE";
            if (stage == EducationStage.University) return "";

            // Secondary: Form 1–3 sit under JC, Form 4–5 under BGCSE.
            return ExtractNumber(levelTitle) >= 4 ? "BGCSE" : "JC";
        }

        static int ExtractNumber(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            foreach (var c in text)
                if (c >= '0' && c <= '9') return c - '0';
            return 0;
        }
    }
}
