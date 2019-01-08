using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitaIMQBot.Bots
{
    public class CitaIMQBotAccessors
    {
        public CitaIMQBotAccessors( ConversationState conversationState, UserState userState )
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        public IStatePropertyAccessor<CitaIMQBotState> CitaIMQState { get; set; }

        public ConversationState ConversationState { get; }
        public UserState UserState { get; }
    }
}
