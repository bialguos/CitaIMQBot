using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Dialogs
{
    public class MainDialog: WaterfallDialog
    {
        public MainDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
        {
            AddStep(async ( stepContext, cancellationToken ) =>
            {
                return await stepContext.PromptAsync("choicePrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("Que te gustaría hacer, coger una cita o consultar tus citas"),
                        RetryPrompt = MessageFactory.Text("No es una opción valida, vuelva a intentarlo por favor."),
                        Choices = new[] { new Choice { Value = "Consultar Citas" }, new Choice { Value = "Coger Una Cita" } }.ToList()
                    });
            });
            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var response = (stepContext.Result as FoundChoice)?.Value;

                if (response == "Consultar Citas")
                {
                    return await stepContext.BeginDialogAsync(ConsultarCitasDialog.Id);
                }

                if (response == "Coger Una Cita")
                {
                    return await stepContext.BeginDialogAsync(CrearCitaDialog.Id);
                }

                return await stepContext.NextAsync();
            });

            AddStep(async ( stepContext, cancellationToken ) => { return await stepContext.ReplaceDialogAsync(Id); });
        }


        public static string Id => "mainDialog";

        public static MainDialog Instance { get; } = new MainDialog(Id);
    }
}
