using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public class PromptModel
    {
        public bool HasConfirmed { get; set; }
        public string Title { get; set; }
        public string Prompt { get; set; }
        public string ConfirmButtonText { get; set; }
        public string DenyButtonText { get; set; }

    }
}
