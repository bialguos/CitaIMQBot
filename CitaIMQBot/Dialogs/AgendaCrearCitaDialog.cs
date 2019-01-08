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
    public class AgendaCrearCitaDialog : WaterfallDialog
    {

        public AgendaCrearCitaDialog( string dialogId, IEnumerable<WaterfallStep> steps = null ) : base(dialogId, steps)
        {

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);

                return await stepContext.PromptAsync("agendaPrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("Introduce el nombre del médico"),
                        RetryPrompt = MessageFactory.Text("No existe ningún médico con ese nombre, vuelve a intentarlo por favor."),

                    });


            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);
                if (state.Agendasposibles.Count() > 1)
                {
                    List<Choice> seleccionagenda = (from p in state.Agendasposibles
                                                    select new Choice
                                                    {
                                                        Value = p
                                                    }).ToList();
                    return await stepContext.PromptAsync("seleccionAgendaPosible",
                        new PromptOptions
                        {
                            Prompt = stepContext.Context.Activity.CreateReply("Seleccione un médico de esta lista"),
                            RetryPrompt = MessageFactory.Text("No es un médico valido, vuelva a introducirlo por favor."),
                            Choices = seleccionagenda,
                            
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
                if (state.Agendasposibles.Count() > 1)
                {
                    var response = (stepContext.Result as FoundChoice)?.Value;
                    var agendaseleccionada = (from p in state.Agendasposibles
                                              where p.ToLower() == response.ToLower()
                                              select p).FirstOrDefault();
                    if (!string.IsNullOrEmpty(agendaseleccionada))
                    {
                        state.Agendasposibles.Clear();
                        state.Agendasposibles.Add(agendaseleccionada);
                        return await stepContext.NextAsync();

                    } else
                    {
                        return await stepContext.ReplaceDialogAsync(AgendaCrearCitaDialog.Id, cancellationToken);

                    }
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);

                return await stepContext.PromptAsync("confirmarAgendaPrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply($"Has elegido {state.Agendasposibles[0]}, es correcto?"),
                        RetryPrompt = MessageFactory.Text(($"Has elegido {state.Agendasposibles[0]}, es correcto?")),
                        Choices = new[] { new Choice { Value = "Si" }, new Choice { Value = "No" } }.ToList()
                    });
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var response = (stepContext.Result as FoundChoice)?.Value;

                if (response == "Si")
                {
                    return await stepContext.NextAsync();
                }

                if (response == "No")
                {
                    return await stepContext.ReplaceDialogAsync(AgendaCrearCitaDialog.Id, cancellationToken);

                }

                return await stepContext.NextAsync();
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {

                    return await stepContext.PromptAsync("fechabusquedaPrompt",
                        new PromptOptions
                        {
                            Prompt = stepContext.Context.Activity.CreateReply($"Introduce la fecha que quieres de la cita"),
                            RetryPrompt = stepContext.Context.Activity.CreateReply($" Introduce de nuevo la fecha que quieres de la cita.")
                        });
               
            });

            AddStep(async ( stepContext, cancellationToken ) =>
            {
                var state = await (stepContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(stepContext.Context);

                    List<DateTimeResolution> fechabusqueda = (List<DateTimeResolution>)stepContext.Result;
                    if (fechabusqueda?.Count() > 0)
                    {
                        state.FechaBusqueda = Convert.ToDateTime(fechabusqueda[0].Value);
                    List<string> huecos = DevolverListaHuecosLibres(state.Agendasposibles[0], state.FechaBusqueda);
                    }
                
                return await stepContext.NextAsync();

            });

           
        }

        private List<string> DevolverListaHuecosLibres( string agenda, DateTime fechaBusqueda )
        {
            List<string> huecoslibres = new List<string>();

            huecoslibres.Add("28/03/2019 10:00 a 12:00");
            huecoslibres.Add("28/03/2019 14:00 a 18:00");
            huecoslibres.Add("29/03/2019 14:00 a 18:00");
            huecoslibres.Add("30/03/2019 14:00 a 18:00");
            return huecoslibres;
        }

        public static string Id => "agendaCrearCitaDialog";

        public static AgendaCrearCitaDialog Instance { get; } = new AgendaCrearCitaDialog(Id);
    }
}
