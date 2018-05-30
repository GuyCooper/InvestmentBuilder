﻿using InvestmentBuilderService.Session;
using System.Timers;
using System;
using Newtonsoft.Json;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Interface defines an updater. this is an object that can send updates to a channel
    /// </summary>
    internal interface IChannelUpdater
    {
        /// <summary>
        /// send an update to the channel
        /// </summary>
        /// <param name="payload"></param>
        void SendUpdate(Dto payload);
        /// <summary>
        /// flag is set when the updater has completed (if ever)
        /// </summary>
        bool Completed { get; }
    }

    /// <summary>
    /// base class for an ChannelUpdater. sends updates to a channel
    /// </summary>
    internal abstract class ChannelUpdater : IChannelUpdater
    {
        #region Pubic Methods
        public ChannelUpdater(IConnectionSession session, string channel, string sourceId)
        {
            _channel = channel;
            _session = session;
            _sourceId = sourceId;
        }
        /// <summary>
        /// send an update to the channel
        /// </summary>
        /// <param name="payload"></param>
        public void SendUpdate(Dto payload)
        {
            _session.SendMessageToChannel(_channel, JsonConvert.SerializeObject(payload), _sourceId, null);
        }

        public bool Completed { get; protected set; }

        #endregion

        #region Private Data Members
        private readonly IConnectionSession _session;
        private readonly string _sourceId;
        private readonly string _channel;
        #endregion
    }
    /// <summary>
    /// this is an abstract class for updating a channel. it contains a timer that can 
    /// be configured to fire at intervals/ Concrete implementations can override the
    /// elapsed callback
    /// </summary>
    internal abstract class TimerUpdater : ChannelUpdater,  IDisposable
    {
        public TimerUpdater(IConnectionSession session, string channel, string sourceId, int intervalMS) 
            : base(session,channel, sourceId)
        {
            _timer = new Timer(intervalMS); //build report timer will update every intervalMS
            _timer.Elapsed += (o, s) =>
            {
                if(OnUpdate() == false)
                {
                    _timer.Stop();
                    Completed = true;
                }
            };
            _timer.AutoReset = true;
            _timer.Enabled = false;
        }

        #region Protected Members
        protected abstract bool OnUpdate();

        /// <summary>
        /// start the timer
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        #endregion
        #region IDisposable
        public void Dispose()
        {
            if(IsDisposed == true)
            {
                return;
            }
            _timer.Dispose();
            IsDisposed = true;
        }

        private bool IsDisposed;
        #endregion

        #region Private Data Members
        private readonly Timer _timer;
        #endregion
    }
}
