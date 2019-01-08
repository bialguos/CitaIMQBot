using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Dialogs
{
    public class SeleccionarAgendaEspecialidadDialog : WaterfallDialog
    {
        public SeleccionarAgendaEspecialidadDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
        {
            AddStep(async ( stepContext, cancellationToken ) =>
            {
                return await stepContext.PromptAsync("seleccionarTipoAgendaEspecialidadPrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("Quieres coger una cita para una especialidad o una agenda concreta?"),
                        RetryPrompt = MessageFactory.Text("No es una opción valida, vuelva a intentarlo por favor."),
                        Choices = new[] { new Choice { Value = "Especialidad" }, new Choice { Value = "Médico" } }.ToList()
                    });
            });
            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var response = (stepContext.Result as FoundChoice)?.Value;

                if (response == "Especialidad"  )
                {
                    return await stepContext.BeginDialogAsync(EspecialidadCrearCitaDialog.Id);
                }

                if (response == "Médico" )
                {
                    return await stepContext.BeginDialogAsync(AgendaCrearCitaDialog.Id);
                }

                return await stepContext.NextAsync();
            });

            AddStep(async ( stepContext, cancellationToken ) => { return await stepContext.ReplaceDialogAsync(Id); });
        }


        public static string Id => "seleccionarAegndaEspecialidadDialog";

        public static SeleccionarAgendaEspecialidadDialog Instance { get; } = new SeleccionarAgendaEspecialidadDialog(Id);
    }
}
