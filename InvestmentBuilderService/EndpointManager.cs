﻿using System.Threading.Tasks;
using Middleware;
using InvestmentBuilderService.Session;
using System;
using Unity;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Abstract EndpointManager class. Defines a base class cfor an endpoint. An endpoint manager
    /// registrs with the connection session and multiplexes messages onto the correct endpoint.
    /// 
    /// </summary>
    internal abstract class EndpointManager : IDisposable
    {
        #region Public Methods

        /// <summary>
        /// Constructor. Inject the Connection session.
        /// </summary>
        public EndpointManager(IConnectionSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Connect to session . Async method.
        /// </summary>
        public virtual async Task<bool> Connect()
        {
            var connected = await _session.Connect();
            if (connected == true)
            {
                _session.RegisterMessageHandler(EndpointMessageHandler);
            }
            return connected;
        }

        /// <summary>
        /// Get the underlying session.
        /// </summary>
        public IConnectionSession GetSession()
        {
            return _session;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abstract method for registering channels.
        /// </summary>
        public abstract void RegisterChannels(IUnityContainer container);

        #endregion

        #region Protected methods

        /// <summary>
        /// Handle a message from the session.
        /// </summary>
        protected abstract void EndpointMessageHandler(Message message);

        #endregion

        #region Private Data

        /// <summary>
        /// Session connection instance.
        /// </summary>
        private IConnectionSession _session;

        #endregion
    }
}
