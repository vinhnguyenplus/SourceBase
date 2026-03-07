<template>
	<div class="mqtt-box">
		<h1 class="header">MQTTX Online Test Client</h1>
		<el-card :model="connection">
			<h1>Connection Parameters (Configuration)</h1>
			<el-form label-position="top" :model="connection">
				<el-row :gutter="6">
					<el-col :span="8">
						<el-form-item prop="host" label="Protocol|Host|Port">
							<el-input v-model="connection.host" :disabled="connSuccess" type="password" show-password>
								<template #prepend>
									<el-select v-model="connection.protocol" class="w80" :disabled="connSuccess" @change="handleProtocolChange">
										<el-option label="ws://" value="ws"></el-option>
										<el-option label="wss://" value="wss"></el-option>
									</el-select>
								</template>
								<template #append>
									<el-input v-model.number="connection.port" type="number" class="w80" :disabled="connSuccess" placeholder="8083/8084"></el-input>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="clientId" label="Uniqueness of Identifier (Client ID)">
							<el-input v-model="connection.clientId"> </el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="username" label="Account(Username)">
							<el-input v-model="connection.username"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="password" label="Password">
							<el-input v-model="connection.password" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="regpacket" label="Device package name (Regpacket)">
							<el-input v-model="connection.repacket" :disabled="connSuccess" @input="syncdhtreg" @change="init_topic"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="dhtRegpack" label="Shared sensor (dhtRegpacket)">
							<el-input v-model="connection.dhtRegpack" :disabled="connSuccess" @change="init_topic"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button
							type="primary"
							:icon="Setting"
							class="sub-btn"
							:disabled="client.connected"
							@click="createConnection"
							:loading="btnLoadingType === 'connect'"
							:style="{ display: client.connected ? 'none' : '' }"
						>
							{{ client.connected ? 'Connected' : 'Connect' }}
						</el-button>
						<el-button v-if="client.connected" class="sub-btn" type="warning" :icon="Discount" @click="destroyConnection" :loading="btnLoadingType === 'disconnect'"> Disconnect </el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>Subscribe</h1>
			<el-form label-position="top" :model="subscription">
				<el-row :gutter="6">
					<el-col :span="12">
						<el-form-item prop="topic" label="Subscribe to topic(Topic)">
							<el-input v-model="connection.subTopics" :disabled="subscribedSuccess" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="qos" label="Quality of Subscription (QoS)">
							<el-select v-model="subscription.qos" :disabled="subscribedSuccess">
								<el-option v-for="qos in qosList" :key="qos" :label="qos == 0 ? '0 at most once' : qos == 1 ? '1 at least once' : '2 exactly once'" :value="qos"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button
							type="primary"
							:icon="Connection"
							class="sub-btn"
							:style="{ display: subscribedSuccess ? 'none' : '' }"
							:loading="btnLoadingType === 'subscribe'"
							:disabled="!client.connected || subscribedSuccess"
							@click="doSubscribe"
						>
							{{ subscribedSuccess ? 'Subscribed' : 'Subscribe' }}
						</el-button>
						<el-button v-if="subscribedSuccess" type="warning" :icon="Discount" class="sub-btn" :loading="btnLoadingType === 'unsubscribe'" :disabled="!client.connected" @click="doUnSubscribe">
							Unsubscribe
						</el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>Publish</h1>
			<el-form label-position="top" :model="publish">
				<el-row :gutter="6">
					<el-col :span="8">
						<el-form-item prop="topic" label="Publish topic(Topic)">
							<el-input v-model="connection.pubTopic" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="qos" label="Release Quality (QoS)">
							<el-select v-model="publish.qos">
								<el-option v-for="qos in qosList" :key="qos" :label="qos == 0 ? '0 at most once' : qos == 1 ? '1 at least once' : '2 exactly once'" :value="qos"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="retain" label="Release Retain">
							<el-select v-model="publish.retain">
								<el-option value="false" label="false does not retain"></el-option>
								<el-option value="true" label="true not retained"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="6">
					<el-col :span="16">
						<el-form-item prop="payload" label="Operation Command (Payload)">
							<el-input v-model="publish.payload" clearable maxlength="64" show-word-limit>
								<!--<template #prepend>
								<el-button :icon="Operation" />
								</template> -->
								<template #append>
									<el-select v-model="publish.payload" placeholder="Select command" style="width: 115px">
										<el-option label="Status query" value="55 AA AA AA AA 91 CF" />
										<el-option label="Open all" value="55 AA AA AA AA 81 A4 01" />
										<el-option label="Turn off all" value="55 AA AA AA AA 81 A4 00" />
										<el-option label="Single-pole switch" value="55 AA AA AA AA 81 BA 01" />
										<el-option label="Two-way switch" value="55 AA AA AA AA 81 BA 02" />
										<el-option label="Three-way switch" value="55 AA AA AA AA 81 BA 03" />
										<el-option label="Fourroadswitch" value="55 AA AA AA AA 81 BA 04" />
									</el-select>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button type="success" :icon="Position" class="sub-btn" :loading="btnLoadingType === 'publish'" :disabled="!client.connected" @click="doPublish(publish.payload, connection.pubTopic)">
							Publish
						</el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>
				<el-button @click="clsmsg" type="success" :icon="Delete" title="Click to clear history">Receive</el-button>
				<el-tag title="Number of receptions">Receive {{ recvnum }}</el-tag>
				<el-tag :title="dht_tm">{{ dht_wsd }}</el-tag>
				<el-tag title="Device operating time">{{ parseInt(runSeconds) }} seconds</el-tag>
				<el-button
					type="success"
					title="close all the way"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch1_Status"
					icon="ele-Check"
					id="ch1"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 01 00')"
					>Close</el-button
				>
				<el-button
					type="warning"
					title="open all the way"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch1_Status"
					icon="ele-CloseBold"
					id="ch1"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 01 01')"
					>Open</el-button
				>
				<el-button
					type="success"
					title="Close second road"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch2_Status"
					icon="ele-Check"
					id="ch2"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 02 00')"
					>Close</el-button
				>
				<el-button
					type="warning"
					title="Turn on channel two"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch2_Status"
					icon="ele-CloseBold"
					id="ch2"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 02 01')"
					>Open</el-button
				>
				<el-button
					type="success"
					title="Turn off three channels"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch3_Status"
					icon="ele-Check"
					id="ch3"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 03 00')"
					>Close</el-button
				>
				<el-button
					type="warning"
					title="open three way"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch3_Status"
					icon="ele-CloseBold"
					id="ch3"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 03 01')"
					>Open</el-button
				>
				<el-button
					type="success"
					title="Close four roads"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch4_Status"
					icon="ele-Check"
					id="ch4"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 04 00')"
					>Close</el-button
				>
				<el-button
					type="warning"
					title="Turn on the four channels"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch4_Status"
					icon="ele-CloseBold"
					id="ch4"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 04 01')"
					>Open</el-button
				>
				<el-button
					type="danger"
					title="All four roads are closed"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.all_Status"
					icon="ele-SwitchButton"
					id="ch5"
					@click="switchLight('55 AA AA AA AA 81 A4 00')"
					>Completely closed</el-button
				>
				<el-button
					type="success"
					title="FourroadOpen all"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.all_Status"
					icon="ele-Switch"
					id="ch5"
					@click="switchLight('55 AA AA AA AA 81 A4 01')"
					>Fully open</el-button
				>

				<el-alert v-if="!client.connected || !connection.onlineStatus" title="Network service disconnected or device offline!" center type="warning" effect="light" style="margin-top: 4px" />
			</h1>
			<!-- Bind to receive log, read-only -->
			<el-col :span="24">
				<el-input type="textarea" :rows="8" id="recv" v-model="receivedMessages" readonly class="log"></el-input>
			</el-col>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="mqttx">
import { reactive, ref, onMounted, nextTick } from 'vue';
import { Search, ChatDotSquare, TopRight, Star, Operation, Setting, Connection, Discount, Open, Delete, Position } from '@element-plus/icons-vue';
//import * as MQTT from 'mqtt/dist/mqtt.min'; // Reference method for version 4.3.7. 5.7.x will prompt an error (import * as MQTT from "mqtt")
import * as MQTT from "mqtt"
import mittBus from '/@/utils/mitt'; // Event bus mitt solves the post-packaging error Uncaught (in promise) ReferenceError: Cannot access 'oe' before initialization

// vue 3 + vite use MQTT.js refer to https://github.com/mqttjs/MQTT.js/issues/1269
// https://github.com/mqttjs/MQTT.js#qos
const qosList = [0, 1, 2]; // quality
const now = new Date();
const recvnum = ref(0);
const dht_wd = ref(0); // temperature, humidity
const dht_sd = ref(0);
const dht_tm = ref(''); // sync time
const dht_wsd = ref('Temperature 0℃, Humidity 0%');
const runSeconds = ref(0); // working hours

// mqtt client variable let or const
const client = ref({
	connected: false, // Not connected
} as MQTT.MqttClient);

const receivedMessages = ref('');
const subscribedSuccess = ref(false); // Subscription success sign
const connSuccess = ref(false); // Connection success sign
const btnLoadingType = ref('');
const retryTimes = ref(0); // Number of reconnections

/**
 * this demo uses EMQX Public MQTT Broker (https://www.emqx.com/en/mqtt/public-mqtt5-broker), here are the details:
 * Referencehttps://github.com/emqx/MQTT-Client-Examples
 * Methodhttps://github.com/mqttjs/MQTT.js
 * Broker host: broker.emqx.io
 * WebSocket port: 8083
 * WebSocket over TLS/SSL port: 8084
 * ws -> 8083; wss -> 8084
 * By default, EMQX allows clients to connect without authentication.
 * https://docs.emqx.com/en/enterprise/v4.4/advanced/auth.html#anonymous-login

 * for more options and details, please refer to https://github.com/mqttjs/MQTT.js#mqttclientstreambuilder-options
 */
const connection = reactive({
	protocol: 'ws',
	host: 'broker.emqx.io',
	// ws -> 8083; wss -> 8084
	port: 8083,
	clientId: 'emqx_vue3_' + Math.random().toString(16).substring(2, 8),
	username: '',
	password: '',
	repacket: 'd1ca1ff51f04', // Registration package (change to your registration package)
	dhtRegpack: 'd1ca1ff51f04', // Temperature registration package (can be the same and share sensors)
	mqttToken: '0804d4c44c1f1bd11dea461481f19868', // Authorize TOKEN to make your own agreement
	keepalive: 30,
	clean: true, // clear clean session
	connectTimeout: 30 * 1000, // ms timeout in milliseconds
	reconnectPeriod: 5000, // ms reconnect milliseconds
	resubscribe: true, // resubscribe
	//Define your own theme
	subTopic: 'mqtt/admintnet/#0#/out',
	willTopic: 'mqtt/admintnet/#0#/will',
	dhtTopic: 'mqtt/admintnet/#0#/dht',
	pubTopic: 'mqtt/admintnet/#0#/into',
	subTopics: [],
	pubPayload: '{"msg":"hellow vue3 mqtt."}',
	onlineStatus: false,
	ch1_Status: false,
	ch2_Status: false,
	ch3_Status: false,
	ch4_Status: false,
	all_Status: false,
	isAC: null, // Strong power true
});
// Initialize theme
const init_topic = () => {
	let st = 'mqtt/admintnet/#0#/out'; // Subscribe to topics
	let pt = 'mqtt/admintnet/#0#/into'; // Post topic
	let ptbody = '{"token":"{0}","cmd":"{1}","cmdpara":"{2}","clientid":"{3}"}';
	let wt = 'mqtt/admintnet/#0#/will'; // Will topics
	let dh = 'mqtt/admintnet/#0#/dht'; // Temperature and humidity
	connection.subTopic = st.replace('#0#', connection.repacket);
	connection.willTopic = wt.replace('#0#', connection.repacket);
	connection.dhtTopic = dh.replace('#0#', connection.dhtRegpack); // Temperature and humidity
	connection.pubTopic = pt.replace('#0#', connection.repacket);
	connection.subTopics = [connection.subTopic, connection.willTopic, connection.dhtTopic];
	connection.pubPayload = ptbody;
	//console.log(connection.subTopics);
};

// By default, the registration package synchronization is consistent with the sensor package name, and vice versa.
const syncdhtreg = () => {
	connection.dhtRegpack = connection.repacket;
};

// String replacement simulation string.format(str,ar1,arn)
const stringFormat = (formatted, args) => {
	for (let i = 0; i < args.length; i++) {
		let regexp = new RegExp('\\{' + i + '\\}', 'gi');
		formatted = formatted.replace(regexp, args[i]);
	}
	return formatted;
};

onMounted(async () => {
	init_topic();
	nextTick(() => {});
});

// topic & QoS for MQTT subscribing Subscribing to topics (multiple)
const subscription = ref({
	topic: `$(connection.subTopics.value)`,
	qos: 0 as MQTT.QoS,
});

// topic, QoS & payload for publishing message publishing topic
const publish = ref({
	topic: `${connection.pubTopic}`,
	qos: 0 as MQTT.QoS,
	retain: false, // Keep No
	payload: '55 AA AA AA AA 91 CF', //'{ "msg": "Hello, I am browser." }',
});

const initData = () => {
	client.value = {
		connected: false,
	} as MQTT.MqttClient;
	retryTimes.value = 0;
	btnLoadingType.value = '';
	subscribedSuccess.value = false;
};

const handleOnReConnect = () => {
	retryTimes.value++;
	connection.clientId = 'emqx_vue3_' + Math.random().toString(16).substring(2, 8);
	console.log(retryTimes.value, 'Number of retries');
	if (retryTimes.value > 5) {
		try {
			client.value.end(); // Disconnected after reconnecting more than 5 times
			initData();
			console.log('connection maxReconnectTimes limit, stop retry');
			appmessage(now.toLocaleString() + '|Exceeded the number of reconnections, stop retrying' + retryTimes.value);
		} catch (error) {
			console.log('handleOnReConnect catch error:', error);
		}
	}
};

/**
 * if protocol is "ws", connectUrl = "ws://broker.emqx.io:8083/mqtt"
 * if protocol is "wss", connectUrl = "wss://broker.emqx.io:8084/mqtt"
 *
 * /mqtt: MQTT-WebSocket uniformly uses /path as the connection path,
 * which should be specified when connecting, and the path used on EMQX is /mqtt.
 *
 * for more details about "mqtt.connect" method & options,
 * please refer to https://github.com/mqttjs/MQTT.js#mqttconnecturl-options
 */
// create MQTT connection create connection
const createConnection = () => {
	try {
		btnLoadingType.value = 'connect';
		const { protocol, host, port, ...options } = connection;
		const connectUrl = `${protocol}:// ${host}:${port}/mqtt`; // form a new connection string
		console.log(connectUrl, 'Connection address');
		client.value = MQTT.connect(connectUrl, options);
		if (client.value.on) {
			// https://github.com/mqttjs/MQTT.js#event-connect
			client.value.on('connect', () => {
				//v5.x  reconnect
				btnLoadingType.value = '';
				connSuccess.value = true; //client.value.connected;
				console.log('connection successful', client.value.connected);
				appmessage(now.toLocaleString() + '|Connection to the service was successful');
			});

			// https://github.com/mqttjs/MQTT.js#event-reconnect reconnect callback
			client.value.on('reconnect', handleOnReConnect);
			// https://github.com/mqttjs/MQTT.js#event-error
			client.value.on('error', (error) => {
				console.log('connection error:', error);
				appmessage(now.toLocaleString() + '|An error occurred:' + error);
			});

			// https://github.com/mqttjs/MQTT.js#event-message receives messages and the processing method is defined separately
			client.value.on('message', (topic: string, message) => {
				//Treatment method
				recvnum.value++; // Accumulated number of receptions
				doAction(topic, message); // deal with
				receivedMessages.value = receivedMessages.value.concat(
					//Concatenate string output
					now.toLocaleString() + ' ' + `${topic}\r\n` + message.toString() + '\r\n'
				);
				// console.log(now.toLocaleString()+`Received message: ${message} from topic: ${topic}`);
				//This method of scrolling works
				nextTick(() => {
					setTimeout(() => {
						syncBottom(); // scroll to bottom
					}, 50);
				});
			});
		}
	} catch (error) {
		btnLoadingType.value = '';
		console.log('mqtt.connect error:', error);
	}
};

// handle events
const doAction = (t, msg) => {
	let res = JSON.parse(msg.toString()); // The json format must be standardized otherwise an error will occur. Double quotes cannot be single quotes;;; is unsafe but powerful eval('(' + message.toString() + ')'); //JSON.parse(message.toString());//json object

	// The message cannot contain '' otherwise an error will occur.
	let regp = res.regpacket; // Registration package received
	let regs = connection.repacket; // Subscription registration package
	let isOK = regp == regs ? true : false; // Is it a message from this device?
	if (!isOK || regp == null) {
		return; // Not discarded
	}

	if (t == connection.dhtTopic) {
		// Temperature and humidity
		let rp = res.regpacket;
		let wd = res.temperature;
		let sd = res.humidity;
		let sj = res.time;
		let sc = res.runsec;
		if (rp != connection.dhtRegpack) {
			// Temperature and humidity package from subscription
			return;
		}
		if (rp != null) {
			dht_wd.value = wd; // In actual application, just replace these 3 variables
			dht_sd.value = sd;
			dht_tm.value = 'Update time:' + sj;
			runSeconds.value = sc;
			dht_wsd.value = 'Temperature:' + dht_wd.value + '℃,humidity:' + dht_sd.value + '%';
			//state.option.title.text="Real-time temperature and humidity change trend chart (run"+parseInt(sc)+"seconds)";
			//updatechart(false);//Real-time data (this method is real-time push, if you use a timer, it will be displayed regularly) updatewsd_time(false)
		}
	}
	if (t == connection.willTopic) {
		// will
		if (res.redata == 'offline') {
			connection.onlineStatus = false;
		} else {
			connection.onlineStatus = true;
		}
	}
	if (t == connection.subTopic) {
		let rp0 = res.regpacket;
		if (rp0 != undefined) {
			if (rp0 == regs) {
				op(res.redata); // The device executes instructions other than to abandon
			}
		}
	}
};

// Handle switch status (customized instructions, need to be modified to your own instructions)
const op = (cmd: any) => {
	if (cmd == '55 AA AA AA AA 82 01 01') {
		connection.ch1_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 01 00') {
		connection.ch1_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 02 01') {
		connection.ch2_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 02 00') {
		connection.ch2_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 03 01') {
		connection.ch3_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 03 00') {
		connection.ch3_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 04 01') {
		connection.ch4_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 04 00') {
		connection.ch4_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 A4 01') {
		connection.ch1_Status = true;
		connection.ch2_Status = true;
		connection.ch3_Status = true;
		connection.ch4_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 A4 00') {
		connection.ch1_Status = false;
		connection.ch2_Status = false;
		connection.ch3_Status = false;
		connection.ch4_Status = false;
	}
	if (cmd == '55 AA AA AA AA 84 AC 01') {
		connection.isAC = true;
	}
	if (cmd == '55 AA AA AA AA 84 AC 00') {
		connection.isAC = false;
	}
	if (connection.ch1_Status && connection.ch2_Status && connection.ch3_Status && connection.ch4_Status) {
		connection.all_Status = true;
	}
	if (!connection.ch1_Status && !connection.ch2_Status && !connection.ch3_Status && !connection.ch4_Status) {
		connection.all_Status = false;
	}
	if (cmd == '55 AA AA AA AA 84 AC 01') {
		connection.isAC = true;
	}
	if (cmd == '55 AA AA AA AA 84 AC 00') {
		connection.isAC = false;
	}
};

// Automatic synchronous scrolling (delayed execution recommended) textarea:any=null
const syncBottom = () => {
	const textarea = document.getElementById('recv');
	if (textarea) {
		textarea.scrollTop = textarea.scrollHeight - 30;
	}
};

// subscribe topic start subscribing
// https://github.com/mqttjs/MQTT.js#mqttclientsubscribetopictopic-arraytopic-object-options-callback
const doSubscribe = () => {
	btnLoadingType.value = 'subscribe';
	const { topic, qos } = subscription.value;
	console.log(connection.subTopics, 'Subscribe to topics');
	client.value.subscribe(connection.subTopics, { qos }, (error: Error, granted: mqtt.ISubscriptionGrant[]) => {
		btnLoadingType.value = '';
		if (error) {
			console.log('subscribe error:', error);
			return;
		}
		subscribedSuccess.value = true; // Subscription successful
		// The connection is successful and the first inquiry command is issued.
		switchLight('55 AA AA AA AA 91 CF'); // Send homepage inquiry instructions
		console.log('Subscription successful', granted);
	});
};

// unsubscribe topic unsubscribe
// https://github.com/mqttjs/MQTT.js#mqttclientunsubscribetopictopic-array-options-callback
const doUnSubscribe = () => {
	btnLoadingType.value = 'unsubscribe';
	const { topic, qos } = subscription.value;
	client.value.unsubscribe(connection.subTopics, { qos }, (error) => {
		btnLoadingType.value = '';
		subscribedSuccess.value = false;
		if (error) {
			console.log('unsubscribe error:', error);
			return;
		}
		console.log(`unsubscribed topic: ${topic}`);
	});
};

// publish messagepublish message
// https://github.com/mqttjs/MQTT.js#mqttclientpublishtopic-message-options-callback
const doPublish = (b, t) => {
	//btnLoadingType.value = "publish";
	const { topic, qos, payload, retain } = publish.value;
	//console.log(t+b,"Publish content")
	let paybody = stringFormat(connection.pubPayload, [connection.mqttToken, b ?? publish.value.payload, '', connection.clientId]); // Standard format payload
	client.value.publish(t ?? connection.pubTopic, paybody, { qos }, (error) => {
		nextTick(() => {
			// Test delay
			setTimeout(() => {
				btnLoadingType.value = '';
			}, 50);
		});
		if (error) {
			appmessage(now.toLocaleString() + '|Publish message error.' + error);
			console.log('publish error:', error);
			return;
		}
	});
};

// Message append message box
const appmessage = (msg) => {
	receivedMessages.value = receivedMessages.value.concat(
		// Concatenate string output
		msg + '\r\n'
	);
};

// switch
const switchLight = (cmd) => {
	if (!client.value.connected) {
		appmessage('Not connected to the service yet!');
		return;
	}
	let paybody = stringFormat(connection.pubPayload, [connection.mqttToken, cmd ?? publish.value.payload, '', connection.clientId]);
	const { topic, qos, payload, retain } = publish.value;
	//console.log(t+b,"Publish content")
	client.value.publish(connection.pubTopic, paybody, { qos }, retain, (error) => {
		btnLoadingType.value = '';
		if (error) {
			console.log('publish error:', error);
			return;
		}
	});
};

// disconnect port connection
// https://github.com/mqttjs/MQTT.js#mqttclientendforce-options-callback
const destroyConnection = () => {
	if (client.value.connected) {
		btnLoadingType.value = 'disconnect';
		try {
			client.value.end(false, () => {
				initData();
				connSuccess.value = false;
				//console.log("disconnected successfully");
				appmessage(now.toLocaleString() + '|Connection has been disconnected.');
			});
		} catch (error) {
			btnLoadingType.value = '';
			console.log('disconnect error disconnect error:', error);
		}
	}
};

// Ports vary by protocol
const handleProtocolChange = (value: string) => {
	connection.port = value === 'wss' ? 8084 : 8083;
};

// Clear message box
const clsmsg = () => {
	receivedMessages.value = '';
};
</script>

<style lang="scss" scoped>
.mqtt-box {
	max-width: 100%;
	margin: 0 auto;
}

.header {
	font-size: 24px;
	font-weight: bold;
	margin: -6px auto 0px auto;
}

h1 {
	font-size: 16px;
	margin-top: 10px auto 20px auto;
	padding: 6px 0px 6px 0;
}

.el-col {
	padding: 4px;
}

.el-input {
	font-size: 13px;
}
.el-card {
	margin-bottom: 6px;
}
.el-card__body {
	padding: 24px;
}

.el-select {
	width: 100%;
}

.text-right {
	text-align: right;
}

.sub-btn {
	margin-top: 20px;
	width: 160px;
}

.hidden {
	display: none;
}
.w80 {
	width: 80px;
}
.w100 {
	width: 100px;
}
.log {
	font-size: 14px;
	color: #fff;
	background-color: black;
}
.center {
	text-align: center;
}
#ch1,
#ch2,
#ch3,
#ch4,
#ch5 {
	width: 120px;
}
.el-tag {
	padding: auto 4px;
	margin: 5px;
	min-width: 60px;
}
</style>
