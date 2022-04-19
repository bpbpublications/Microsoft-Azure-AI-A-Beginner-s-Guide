// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.AI.QnA;

namespace OrchestratorBot
{
    public interface IBotServices
    {
        LuisRecognizer LuisReservationRecognizer { get; }
        
        OrchestratorRecognizer Dispatch { get; }
        
        QnAMaker McDonaldsQnA { get; }
    }
}
