using CitaIMQBot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Dialogs
{
    public class ConsultarCitasDialog : WaterfallDialog
    {
        public ConsultarCitasDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
        {

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);
                if (state.NumeroTarjeta == 0)
                {
                    return await stepContext.PromptAsync("numeroTarjeta",
                        new PromptOptions
                        {
                            Prompt = stepContext.Context.Activity.CreateReply("Cual es tu número de tarjeta?"),
                            RetryPrompt = MessageFactory.Text("No es un número de tarjeta valido, vuelva a introducirlo por favor."),

                        });
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);
                if (state.NumeroTarjeta == 0)
                {
                    state.NumeroTarjeta = (Int32)stepContext.Result;

                }
                if (state.FechaNacimiento == DateTime.MinValue)
                {

                    return await stepContext.PromptAsync("fechanacimientoPrompt",
                        new PromptOptions
                        {
                            Prompt = stepContext.Context.Activity.CreateReply($"{state.Nombre}, introduce la fecha de nacimiento?"),
                            RetryPrompt = stepContext.Context.Activity.CreateReply($"{state.Nombre}, Introduce de nuevo la fecha de nacimiento.")
                        });
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);

                if (state.FechaNacimiento == DateTime.MinValue)
                {

                    List<DateTimeResolution> fechanacimiento = (List<DateTimeResolution>)stepContext.Result;
                    if (fechanacimiento?.Count() > 0)
                    {
                        state.FechaNacimiento = Convert.ToDateTime(fechanacimiento[0].Value);

                    }
                }
                await stepContext.Context.SendActivityAsync($"Muchas gracias {state.Nombre}, tus citas son las siguientes");
                if (state.NumeroTarjeta.ToString().StartsWith("10"))
                {
                    await stepContext.Context.SendActivityAsync($"28/02/2019 12:30 Dra Boyero"
                        + Environment.NewLine +
                        "28/03/2019 12:30 Dra Aya");

                }
                else if (state.NumeroTarjeta.ToString().StartsWith("11"))
                {
                    await stepContext.Context.SendActivityAsync($"03/02/2019 12:30 Dra Boyero"
                        + Environment.NewLine +
                        "203/03/2019 12:30 Dra Aya");

                }
                await stepContext.Context.SendActivityAsync($"Gracias por haber utilizado el bot de citación");
                return await stepContext.EndDialogAsync();
            });

        }
        public static string Id => "consultarCitasDialog";

        public static ConsultarCitasDialog Instance { get; } = new ConsultarCitasDialog(Id);
    }
}
