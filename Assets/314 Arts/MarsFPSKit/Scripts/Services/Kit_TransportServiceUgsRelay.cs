using kcp2k;
using Mirror;
using UnityEngine;
using Utp;

namespace MarsFPSKit
{
    namespace Services
    {
        /// <summary>
        /// Implements unity relay via UtpTransport. Acts similar to Photon!
        /// </summary>
        [CreateAssetMenu(menuName = "MarsFPSKit/Services/Transport/Unity Relay")]
        public class Kit_TransportServiceUgsRelay : Kit_TransportServiceBase
        {
            public bool simulateLatency;

            private string currentRelayJoinCode;

            public override void ConnectWithString(Kit_NetworkManager manager, string connectionString)
            {
                UtpTransport transport = manager.GetComponent<UtpTransport>();
                transport.ConfigureClientWithJoinCode(connectionString,
                () =>
                {
                    manager.StartClient();
                },
                () =>
                {
                    UtpLog.Error($"Failed to join Relay server.");
                });
            }

            public override void ConnectWithStringAndPassword(Kit_NetworkManager manager, string connectionString, string password)
            {
                base.ConnectWithStringAndPassword(manager, connectionString, password);

                UtpTransport transport = manager.GetComponent<UtpTransport>();
                transport.ConfigureClientWithJoinCode(connectionString,
                () =>
                {
                    manager.StartClient();
                },
                () =>
                {
                    UtpLog.Error($"Failed to join Relay server.");
                });
            }

            public override string GetConnectionString(Kit_NetworkManager manager)
            {
                return currentRelayJoinCode;
            }

            public override void Initialize(Kit_NetworkManager manager)
            {
                if (simulateLatency)
                {
                    LatencySimulation ls = manager.gameObject.AddComponent<LatencySimulation>();
                    UtpTransport transport = manager.gameObject.AddComponent<UtpTransport>();
                    ls.wrap = transport;
                    manager.transport = ls;
                    transport.useRelay = true;
                    Transport.active = ls;
                    ls.enabled = true;
                    ls.reliableLatency = 0.3f;
                    ls.unreliableLatency = 0.3f;
                    ls.unreliableLoss = 0.3f;
                    ls.unreliableScramble = 0.3f;
                }
                else
                {
                    UtpTransport transport = manager.gameObject.AddComponent<UtpTransport>();
                    manager.transport = transport;
                    transport.useRelay = true;
                    Transport.active = transport;
                }
            }

            public override void StartServer(Kit_NetworkManager manager)
            {
                UtpTransport transport = manager.GetComponent<UtpTransport>();

                if (!transport)
                {
                    //In case that offline mode was previously on

                    //Destroy transports
                    Transport[] transports = manager.gameObject.GetComponents<Transport>();

                    for (int i = 0; i < transports.Length; i++)
                    {
                        Destroy(transports[i]);
                    }

                    Initialize(manager);
                    transport = manager.GetComponent<UtpTransport>();
                }

                transport.AllocateRelayServer(manager.maxConnections, null, (string joinCode) =>
                {
                    currentRelayJoinCode = joinCode;
                    manager.StartServer();
                },
                () =>
                {
                    UtpLog.Error($"Failed to start a Relay host.");
                });
            }

            public override void StartHost(Kit_NetworkManager manager)
            {
                UtpTransport transport = manager.GetComponent<UtpTransport>();

                if (!transport)
                {
                    //In case that offline mode was previously on

                    //Destroy transports
                    Transport[] transports = manager.gameObject.GetComponents<Transport>();

                    for (int i = 0; i < transports.Length; i++)
                    {
                        Destroy(transports[i]);
                    }

                    Initialize(manager);
                    transport = manager.GetComponent<UtpTransport>();
                }

                transport.AllocateRelayServer(manager.maxConnections, null, (string joinCode) =>
                {
                    currentRelayJoinCode = joinCode;
                    manager.StartHost();
                },
                () =>
                {
                    UtpLog.Error($"Failed to start a Relay host.");
                });
            }
        }
    }
}