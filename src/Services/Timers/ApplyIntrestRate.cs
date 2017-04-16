﻿using DEA.Database.Models;
using MongoDB.Driver;
using System.Threading;

namespace DEA.Services.Timers
{
    class ApplyIntrestRate
    {
        private Timer _timer;

        public ApplyIntrestRate()
        {
            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.INTEREST_RATE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var builder = Builders<Gang>.Filter;
            var updateBuilder = Builders<Gang>.Update;
            foreach (var gang in await(await DEABot.Gangs.FindAsync(builder.Empty)).ToListAsync())
                if (gang.Wealth < 1000000000000000000000m)
                    await DEABot.Gangs.UpdateOneAsync(y => y.Id == gang.Id,
                        updateBuilder.Set(x => x.Wealth, Math.CalculateIntrestRate(gang.Wealth) * gang.Wealth + gang.Wealth));
        }
    }
}