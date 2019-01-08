using CitaIMQBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CitaIMQBot.Bots
{
    public class CitaIMQBot : IBot
    {
        private readonly DialogSet _dialogs;
        private readonly CitaIMQBotAccessors _accessors;
        public CitaIMQBot( CitaIMQBotAccessors accessors )
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
            _dialogs = new DialogSet(accessors.DialogStateAccessor);
            var dialogState = accessors.DialogStateAccessor;

            _dialogs = new DialogSet(dialogState);
            _dialogs.Add(MainDialog.Instance);
            _dialogs.Add(ConsultarCitasDialog.Instance);
            _dialogs.Add(CrearCitaDialog.Instance);
            _dialogs.Add(SeleccionarAgendaEspecialidadDialog.Instance);
            _dialogs.Add(AgendaCrearCitaDialog.Instance);
            _dialogs.Add(EspecialidadCrearCitaDialog.Instance);

            _dialogs.Add(new ChoicePrompt("choicePrompt"));
            _dialogs.Add(new NumberPrompt<int>("numeroTarjeta",ValidarTarjetaAsync));
            _dialogs.Add(new DateTimePrompt("fechanacimientoPrompt", ValidarFechaNacimientoAsync,Culture.Spanish));
            _dialogs.Add(new ChoicePrompt("seleccionarTipoAgendaEspecialidadPrompt"));
            _dialogs.Add(new ChoicePrompt("seleccionarProvinciaPrompt"));
            _dialogs.Add(new TextPrompt("agendaPrompt", ValidarAgendaPrompt));
            var choiceagendasposibles = new ChoicePrompt("seleccionAgendaPosible");
            choiceagendasposibles.Style = Microsoft.Bot.Builder.Dialogs.Choices.ListStyle.Auto;
            _dialogs.Add(choiceagendasposibles);
            _dialogs.Add(new ChoicePrompt("confirmarAgendaPrompt"));
            _dialogs.Add(new DateTimePrompt("fechabusquedaPrompt", ValidarFechaBusquedaAsync,Culture.Spanish));


        }

        private async Task<bool> ValidarFechaBusquedaAsync( PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken )
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Lo siento no es una fecha correcta.",
                    cancellationToken: cancellationToken);
                return false;
            }


            var fechabusqueda = promptContext.Recognized.Value;
            if (fechabusqueda?.Count() > 0)
            {
                if (Convert.ToDateTime(fechabusqueda[0].Value) <= DateTime.Now)
                {
                    await promptContext.Context.SendActivityAsync(
                  "No puedes introducir una fecha menor o igual que la actual.",
                  cancellationToken: cancellationToken);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> ValidarAgendaPrompt( PromptValidatorContext<string> promptContext, CancellationToken cancellationToken )
        {
            var state = await (promptContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(promptContext.Context);

            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Introduzca el nombre de un médico.",
                    cancellationToken: cancellationToken);
                return false;
            }
            state.Agendasposibles = RecuperarAgendasPosibles(promptContext.Recognized.Value);
            if (state.Agendasposibles?.Count == 0)
            {
                await promptContext.Context.SendActivityAsync(
                    "No existe ningún médico con ese nombre.",
                    cancellationToken: cancellationToken);
                return false;
            }
            return true;
        }

        private List<string> RecuperarAgendasPosibles( string value )
        {
            List<string> agendasposibles = new List<string>();
            if (value.ToLower().Contains("bo"))
            {
                agendasposibles.Add("Dra. Boyero");
                agendasposibles.Add("Dr. Boyerito");
                agendasposibles.Add("Dra. Alama Boyero");
            }
            if (value.ToLower().Contains("aya"))
            {
                agendasposibles.Add("Dr. Aya");
            }
            return agendasposibles;
        }

        private async Task<bool> ValidarFechaNacimientoAsync( PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken )
        {

            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Lo siento no es una fecha correcta.",
                    cancellationToken: cancellationToken);
                return false;
            }


            var fechanacimiento = promptContext.Recognized.Value;
            if (fechanacimiento?.Count()>0)
            {
                if (Convert.ToDateTime(fechanacimiento[0].Value) > DateTime.Now)
                {
                    await promptContext.Context.SendActivityAsync(
                  "No puedes introducir una fecha mayor que la actual.",
                  cancellationToken: cancellationToken);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> ValidarTarjetaAsync( PromptValidatorContext<int> promptContext, CancellationToken cancellationToken )
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Lo siento no es un número lo que has introducido, vuelve a intentarlo.",
                    cancellationToken: cancellationToken);
                return false;
            }
            int numerotarjeta = promptContext.Recognized.Value;
            if (!numerotarjeta.ToString().StartsWith("1"))
            {
                return false;
            }
            else if (numerotarjeta.ToString().StartsWith("10"))
            {
                var state = await (promptContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(promptContext.Context);
                state.Nombre ="Oscar Alvarez Guerras";
            }
            else if (numerotarjeta.ToString().StartsWith("11"))
            {
                var state = await (promptContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(promptContext.Context);
                state.Nombre = "Iñigo Herrero Gutierrez del Anillo";
            } else
            {
                var state = await (promptContext.Context.TurnState["_accessors"] as CitaIMQBotAccessors).CitaIMQState.GetAsync(promptContext.Context);
                state.Nombre = "Desconocido";
            }
            return true;
        }

        public async Task OnTurnAsync( ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken) )
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var dialogCtx = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

                string textointroducido = turnContext.Activity.Text.Trim().ToLowerInvariant();
                if (textointroducido == "cancelar")
                {
                    // Cancel any dialog on the stack.
                    await turnContext.SendActivityAsync("Comencemos de nuevo !!!.", cancellationToken: cancellationToken);
                    await dialogCtx.CancelAllDialogsAsync(cancellationToken);
                }

                // initialize state if necessary
                var state = await _accessors.CitaIMQState.GetAsync(turnContext, () => new CitaIMQBotState(), cancellationToken);

                turnContext.TurnState.Add("_accessors", _accessors);


                if (dialogCtx.ActiveDialog == null)
                {
                    if (textointroducido == "hola")
                    {
                        await dialogCtx.BeginDialogAsync(MainDialog.Id, cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync("Recuerda, escribe hola para comenzar.", cancellationToken: cancellationToken);

                    }
                }
                else
                {
                    await dialogCtx.ContinueDialogAsync(cancellationToken);
                }

                await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate) // Greet when users are added to the conversation.
            {
                if (turnContext.Activity.MembersAdded.Any())
                {
                    await BienvenidaAsync(turnContext, cancellationToken);
                }
            }
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Save the user profile updates into the user state.
            await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private static async Task BienvenidaAsync( ITurnContext turnContext, CancellationToken cancellationToken )
        {
            // Iterate over all new members added to the conversation
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message
                // the 'bot' is the recipient for events from the channel,
                // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                // bot was added to the conversation.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Bienvenido al bot de citación de IMQ", cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync($"Para iniciar la conversación escriba hola, si en cualquier momento quieres reinicciar la conversación o cancelarla escribe cancelar", cancellationToken: cancellationToken);
                }
            }
        }

        
    }
}
