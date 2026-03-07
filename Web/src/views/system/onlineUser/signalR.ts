import * as SignalR from '@microsoft/signalr';
import { ElNotification } from 'element-plus';
import { getToken } from '/@/utils/axios-utils';

// Initialize the SignalR object
const connection = new SignalR.HubConnectionBuilder()
	.configureLogging(SignalR.LogLevel.Information)
	.withUrl(`${window.__env__.VITE_API_URL}/hubs/onlineUser?token=${getToken()}`, { transport: SignalR.HttpTransportType.WebSockets, skipNegotiation: true })
	.withAutomaticReconnect({
		nextRetryDelayInMilliseconds: () => {
			return 5000; // Reconnect every 5 seconds
		},
	})
	.build();

// Heartbeat detection: If no message is sent to the server within 15 seconds, ping the server
connection.keepAliveIntervalInMilliseconds = 15 * 1000;
// Timeout: If no information is received from the server within 30 seconds, the server is considered abnormal.
connection.serverTimeoutInMilliseconds = 30 * 1000;

// Start connection
connection.start().then(() => {
	console.log('Start connection');
});
// Disconnect
connection.onclose(async () => {
	console.log('Disconnect');
});
// Reconnecting
connection.onreconnecting(() => {
	ElNotification({
		title: 'Prompt',
		message: 'The server has been disconnected...',
		type: 'error',
		position: 'bottom-right',
	});
});
// Reconnection successful
connection.onreconnected(() => {
	console.log('Reconnection successful');
});

connection.on('OnlineUserList', () => {});

export { connection as signalR };
