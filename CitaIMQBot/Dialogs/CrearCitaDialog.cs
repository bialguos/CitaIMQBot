using CitaIMQBot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Dialogs
{
    public class CrearCitaDialog : WaterfallDialog
    {
        public CrearCitaDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
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
                return await stepContext.NextAsync();





            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                return await stepContext.PromptAsync("seleccionarProvinciaPrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("En que provincia quieres ir?"),
                        RetryPrompt = MessageFactory.Text("No es una provincia valida, seleccione una de ellas."),
                        Choices = Enum.GetValues(typeof(ProvinciaEnum))
                                                .Cast<ProvinciaEnum>()
                                                .Select(t => new Choice
                                                {
                                                    Value = t.ToString()

                                                }).ToList()
                    });

            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);

                var response = (stepContext.Result as FoundChoice)?.Value;

                if (response.ToLower() == ProvinciaEnum.Bizkaia.ToString().ToLower())
                {
                    state.Provincia = ProvinciaEnum.Bizkaia;
                }

                else if (response.ToLower() == ProvinciaEnum.Araba.ToString().ToLower())
                {
                    state.Provincia = ProvinciaEnum.Araba;

                }
                else if (response.ToLower() == ProvinciaEnum.Gipuzkoa.ToString().ToLower())
                {
                    state.Provincia = ProvinciaEnum.Gipuzkoa;

                }

                return await stepContext.BeginDialogAsync(SeleccionarAgendaEspecialidadDialog.Id);

            });
        }
        public static string Id => "crearCitaDialog";

        public static CrearCitaDialog Instance { get; } = new CrearCitaDialog(Id);
    }
}
