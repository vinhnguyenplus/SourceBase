<template>
	<div class="sys-video-container">
		<el-container>
			<el-header>Video surveillance (EZVIZ Live)</el-header>
			<el-container>
				<el-aside width="200px">
					<el-tree :data="data" :props="defaultProps" @node-click="handleNodeClick" />
				</el-aside>

				<el-main>
					<div class="updateToken" props="ezviz_video">
						<ul>
							<li>
								<el-span>Keychain:</el-span>
								<el-input
									placeholder="Key"
									show-word-limit
									type="text"
									id="txt_token"
									title="Weekly updates (Open Platform, Cloud Live, Lightweight Applications, Code Examples)"
									v-model="ezviz_video.ezvizToken"
									@keyup.enter="update_Token"
									class="token_input"
								/>
							</li>
							<li>
								<el-span>Video stream:</el-span>
								<el-input
									placeholder="Ezviz Cloud Video Stream Address"
									show-word-limit
									type="text"
									id="txt_url"
									title="Video stream address corresponding to the key (HD suffix .h.live)"
									v-model="ezviz_video.ezvizUrl"
									@keyup.enter="update_Token"
									class="token_input"
								/>
							</li>
						</ul>
					</div>

					<div class="video">
						<div class="video-item">
							<div class="item">
								<div class="home" ref="viewtoolOne">
									<div id="video-container">Waiting to load...</div>
								</div>
							</div>
						</div>
					</div>
				</el-main>
			</el-container>
		</el-container>
	</div>
</template>

<!-- 
Warehouse：https://github.com/Ezviz-OpenBiz/EZUIKit-JavaScript-npm
Install：npm install ezuikit-js or pnpm add ezuikit-js
-->
<script lang="ts" setup name="video">
import { reactive, ref, onMounted, nextTick, beforeDestroy } from 'vue';
import EZUIKit from 'ezuikit-js'; // Page reference
//import { ElNotification } from 'element-plus';
//import { Search,ChatDotSquare,TopRight,Star,Operation,Setting,Connection,Discount,Open,Delete,Position,View,CopyDocument,DocumentChecked,VideoCamera} from '@element-plus/icons-vue';
import mittBus from '/@/utils/mitt'; // Event bus mitt solves the post-packaging error Uncaught (in promise) ReferenceError: Cannot access 'oe' before initialization

let ezvizPlayOne = ref(null);
let ezvizPlayTwo = ref(null);
let ezvizPlayThree = ref(null);
let ezvizPlayFour = ref(null);
let viewtoolOne = ref();
let viewtoolTwo = ref();
let viewtoolThree = ref();
let viewtoolFour = ref();

interface Tree {
	label: string;
	children?: Tree[];
}

const defaultProps = {
	children: 'children',
	label: 'label',
};

const handleNodeClick = (data: Tree) => {
	console.log(data);
};

// update token
function update_Token(e) {
	//ezviz_video.ezvizToken=e.target.value;
	console.log(e.target.value);
	autoVideoOne('video-container');
}

onMounted(async () => {
	autoVideoOne('video-container');
	//console.log('https://open.ys7.com/console/ezuikit/template/detail.html?themeId=pcLive&editing=false');
});

// Test ezopen://open.ys7.com/G39444019/1.live and at.3bvmj4ycamlgdwgw1ig1jruma0wpohl6-48zifyb39c-13t5am6-yukyi86mz
// Alternate ezopen://open.ys7.com/AA2615287/1.live and ra.5k88qgc34vgr9yva7rlub985blo9ph7k-92q0bl2r4r-0aygaog-5cofhebpm
const ezviz_video = reactive({
	ezvizToken: 'ra.5k88qgc34vgr9yva7rlub985blo9ph7k-92q0bl2r4r-0aygaog-5cofhebpm', // Need to modify weekly (open platform, cloud live broadcast, light application, code samples can be found) demonstration equipment
	ezvizUrl: 'ezopen:// open.ys7.com/AA2615287/1.live', //HD live broadcast splicing string cosnt url = `ezopen://${item.identifyingCode}@open.ys7.com/${item.imei}/${item.channelNo}.hd.live`
	// Playback address ezopen://open.ys7.com/AA2615287/1.rec
});

// beforeDestroy(()=>{
// 	ezvizPlayOne.value && ezvizPlayOne.value.stop() //Destroy and stop the live video
// 	console.log('beforeDestroy');
// });

// Monitoring 1, parameters https://blog.csdn.net/weixin_53791978/article/details/126489296
function autoVideoOne(params) {
	// Get the width and height of the parent node
	let divW = viewtoolOne.value.clientWidth;
	let divH = viewtoolOne.value.clientHeight;
	if (ezvizPlayOne.value != null) {
		return;
	}

	// Get fluorite token
	ezvizPlayOne.value = new EZUIKit.EZUIKitPlayer({
		autoplay: true, // Play by default
		// Video playback includes elements
		id: 'video-container', // DIV container
		// EZVIZ token, query the example code at https://open.ys7.com/console/ezuikit/template/detail.html?themeId=pcLive&editing=false
		accessToken: ezviz_video.ezvizToken, //"ra.bl9n4hmb3c7w4fk6bbuumtmdcbbo66w0-3k7nal0q6y-0lp00m5-fi61isesz",
		// ezopen://open.ys7.com/${device No}/{channel number}.live
		url: ezviz_video.ezvizUrl, // "ezopen://open.ys7.com/AA2615287/1.live", // playback address
		template: 'standard', // pcLive, simple - minimalist version; standard - standard version; security - security version (preview playback); voice - voice version; theme - configurable theme;
		useHardDev: true, // Turn on high-performance mode. Dependencies need to be higher than 7.7.x. As of 2023.11.7, it is recommended to keep the latest version 7.7.6.
		// header: ['capturePicture', 'zoom'], // If the template parameter is not simple, this field will be overwritten
		//plugin: ['talk'], // Load plugin, talk-talk
		// Bottom controls below the video
		//footer: ["talk", "broadcast", "hd", "fullScreen"], // If the template parameter is not simple, this field will be overwritten
		footer: ['talk', 'hd', 'fullScreen'], // If the template parameter is not simple, this field will be overwritten
		//audio: 0, // Whether to turn on sound by default 0 - off 1 - on
		// openSoundCallBack: data => console.log("Open sound callback", data),
		// closeSoundCallBack: data => console.log("Close sound callback", data),
		// startSaveCallBack: data => console.log("Start recording callback", data),
		// stopSaveCallBack: data => console.log("Recording callback", data),
		// capturePictureCallBack: data => console.log("Screenshot successful callback", data),
		// fullScreenCallBack: data => console.log("Full screen callback", data),
		// getOSDTimeCallBack: data => console.log("Get OSDTime callback", data),
		width: divW,
		height: divH,
		handleError: (err: any) => {
			if (err.type === 'handleRunTimeInfoError' && err.data.nErrorCode === 5) {
				console.log('Encryption devicepasswordmistake');
			}
		},
	});
}

const data: Tree[] = [
	{
		label: 'Node A',
		children: [
			{
				label: 'Menu A-1',
				children: [
					{
						label: 'Menu A-1-1',
					},
				],
			},
		],
	},
	{
		label: 'Node B',
		children: [
			{
				label: 'Menu B-1',
				children: [
					{
						label: 'Menu B-1-1',
					},
				],
			},
			{
				label: 'Menu B-2',
				children: [
					{
						label: 'Menu B-2-1',
					},
				],
			},
		],
	},
	{
		label: 'Node C',
		children: [
			{
				label: 'Menu C-1',
				children: [
					{
						label: 'Menu C-1-1',
					},
				],
			},
			{
				label: 'Menu C-2',
				children: [
					{
						label: 'Menu C-2-1',
					},
				],
			},
		],
	},
];
</script>

<style lang="scss" scoped>
.sys-video-container {
	overflow: hidden;
	height: 100vh;
}
.common-layout {
	background-color: #ecf5ff;
}

.el-header {
	text-align: center;
	height: 45px;
	line-height: 45px;
	font-size: 22px;
	background-color: #eee;
	padding: 5px auto;
}
.el-aside {
	text-align: center;
	padding: 4px auto;
	overflow: hidden;
}
.el-aside .el-form {
	text-align: center;
	padding: 2px auto;
	margin: 4px;
}
.el-aside .el-button {
	margin: 2px;
}
.el-aside .el-input {
	font-size: 14px;
	padding: 2px;
}
.el-aside .el-card {
	margin: 10px 0 10px auto;
}
.el-aside .el-card .el-button {
	width: 90px;
}
.el-form-item {
	font-weight: bold;
}

.el-main {
	background-color: #111;
	padding: 4px;
	width: 100%;
	overflow: hidden;
	color: #fff;
}

.recvs {
	overflow-y: auto;
	overflow-x: hidden;
	width: 100%;
	height: 800px;
}
.rev_title {
	width: 100%;
	display: block;
	font-style: italic;
	color: #999;
	font-size: 14px;
	background-color: #fafafa;
	padding: 2px 4px;
	line-height: 25px;
}
.rev_conts {
	width: 100%;
	word-wrap: break-word;
	line-height: 1.5em;
	padding: 4px;
	line-height: 30px;
} /*Indentationtext-indent:2em;*/
.recvfontsize {
	text-align: center;
	display: block;
	padding-top: 4px;
}
.el-color-picker {
	margin-left: 4px;
}
.recv_count {
	text-align: left;
}
.recv_count p {
	line-height: 30px;
}

.header {
	font-size: 24px;
	font-weight: bold;
	margin: -10px auto 10px auto;
}

h1 {
	font-size: 16px;
	margin-top: 10px auto 20px auto;
	padding: 5px 0px 5px 0;
}

.el-col {
	padding: 4px;
}

.el-input {
	font-size: 13px;
}
.el-card {
	margin-bottom: 12px;
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
	margin-top: 30px;
}

.updateToken {
	display: block;
	line-height: 30px;
	align: left;
	background: rgb(250, 250, 250, 0.2);
	padding: 5px;
}

.token_input {
	width: 90%;
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

el-tree span {
	line-height: 50px;
}

.video {
	width: 100%;
	height: 100%;
	overflow: hidden;

	.video-item {
		display: flex;
		padding: 5px;
		overflow: hidden;

		.item {
			flex: 1;

			min-height: 40%;
			width: 100%;
			margin: 0 5px;
			background-color: #000000;
			color: #fff;
			border-radius: 2px;

			.home {
				width: 100%;
				height: 70vh;
				overflow: hidden;
				padding: 0;
				marigin: 0;
				aspect-ratio: 16/9; /* Set any width and any heightoneitem available Then useaspect-ratioYuanplain Dynamically set scale */
				text-align: center;
				justify-content: center;
			}
		}
	}
}
</style>
