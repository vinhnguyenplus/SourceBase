<template>
	<div class="sys-server-container">
		<el-row :gutter="8">
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="System information">
					<table class="sysInfo_table">
						<tbody>
							<tr>
								<td class="sysInfo_td">Host name:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.hostName }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">Operating system:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.systemOs }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">System architecture:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.osArchitecture }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">CPU cores:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.processorCount }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">Runtime:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.sysRunTime }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">External network address:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.remoteIp }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">Intranet address:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.localIp }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">Run the framework:</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.frameworkDescription }}</td>
							</tr>
						</tbody>
					</table>
				</el-card>
			</el-col>
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="Use information">
					<el-row>
						<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(state.machineUseInfo.ramRate == undefined ? 0 : state.machineUseInfo.ramRate.substr(0, state.machineUseInfo.ramRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ state.machineUseInfo.ramRate }}<br /></span>
									<span style="font-size: 10px">
										Used: {{ state.machineUseInfo.usedRam }}<br />
										Remaining: {{ state.machineUseInfo.freeRam }}<br />
										Memory Usage
									</span>
								</template>
							</el-progress>
						</el-col>
						<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(state.machineUseInfo.cpuRate == undefined ? 0 : state.machineUseInfo.cpuRate.substr(0, state.machineUseInfo.cpuRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ state.machineUseInfo.cpuRate }}<br /></span>
									<span style="font-size: 10px"> CPU Usage </span>
								</template>
							</el-progress>
						</el-col>
					</el-row>

					<el-row>
						<table class="sysInfo_table">
							<tbody>
								<tr>
									<td class="sysInfo_td">Start Time:</td>
									<td class="sysInfo_td">{{ state.machineUseInfo.startTime }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">Runtime:</td>
									<td class="sysInfo_td">{{ state.machineUseInfo.runTime }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">Website directory:</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.wwwroot }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">Development Environment:</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.environment }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">Environment variables:</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.stage }}</td>
								</tr>
							</tbody>
						</table>
					</el-row>
				</el-card>
			</el-col>
		</el-row>

		<el-row :gutter="8">
			<el-col :md="24" :sm="24">
				<el-card shadow="hover" header="Assembly information" style="margin-top: 5px; --el-card-padding: 10px">
					<div v-for="d in state.assemblyInfo" :key="d.name" style="display: inline-block; margin: 4px; text-align: left">
						<el-tag round>
							<div style="display: inline-flex">
								<div>{{ d.name }}</div>
								<div style="color: black; font-size: 9px; margin-left: 3px">v{{ d.version }}</div>
							</div>
						</el-tag>
					</div>
				</el-card>
			</el-col>
		</el-row>

		<el-row :gutter="8">
			<el-col :md="24" :sm="24">
				<el-card shadow="hover" header="disk information" style="margin-top: 5px">
					<el-row>
						<el-col
							:span="4"
							:xs="(24 / state.machineDiskInfo.length) * 2"
							:sm="24 / state.machineDiskInfo.length"
							:md="24 / state.machineDiskInfo.length"
							:lg="24 / state.machineDiskInfo.length"
							:xl="24 / state.machineDiskInfo.length"
							v-for="d in state.machineDiskInfo"
							:key="d.diskName"
							style="text-align: center"
						>
							<el-progress type="circle" :percentage="d.availablePercent" :color="'var(--el-color-primary)'">
								<template #default>
									<span>{{ d.availablePercent }}%<br /></span>
									<span style="font-size: 10px">
										Used: {{ d.used }}GB<br />
										Remaining:{{ d.availableFreeSpace }}GB<br />
										{{ d.diskName }}
									</span>
								</template>
							</el-progress>
						</el-col>
					</el-row>
				</el-card>
			</el-col>
		</el-row>
	</div>
</template>

<script lang="ts" setup name="sysServer">
import { onActivated, onDeactivated, onMounted, reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysServerApi } from '/@/api-services';

const state = reactive({
	machineBaseInfo: [] as any,
	machineUseInfo: [] as any,
	machineDiskInfo: [] as any,
	assemblyInfo: [] as any,
	timer: null as any,
});

onMounted(async () => {
	loadMachineBaseInfo();
	loadMachineUseInfo();
	loadMachineDiskInfo();
	loadAssemblyInfo();
});

// Server configuration information
const loadMachineBaseInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerServerBaseGet();
	state.machineBaseInfo = res.data.result;
};

// Server memory information
const loadMachineUseInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerServerUsedGet();
	state.machineUseInfo = res.data.result;
};

// Server disk information
const loadMachineDiskInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerServerDiskGet();
	state.machineDiskInfo = res.data.result;
};

// Framework assembly information
const loadAssemblyInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerAssemblyListGet();
	state.assemblyInfo = res.data.result;
};

// Refresh memory in real time
const refreshData = () => {
	loadMachineUseInfo();
};

onActivated(() => {
	state.timer = setInterval(() => {
		refreshData();
	}, 10000);
});

onDeactivated(() => {
	clearInterval(state.timer);
});
</script>

<style lang="scss" scoped>
.sysInfo_table {
	width: 100%;
	min-height: 40px;
	line-height: 40px;
	text-align: center;
}

.sysInfo_td {
	border-bottom: 1px solid #e8e8e8;
	min-width: 100px;
}
</style>
