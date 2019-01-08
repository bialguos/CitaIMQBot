using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Dialogs
{
    public class EspecialidadCrearCitaDialog : WaterfallDialog
    {

        public EspecialidadCrearCitaDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
        {

        }

        public static string Id => "sspecialidadCrearCitaDialog";

        public static EspecialidadCrearCitaDialog Instance { get; } = new EspecialidadCrearCitaDialog(Id);
    }
}
