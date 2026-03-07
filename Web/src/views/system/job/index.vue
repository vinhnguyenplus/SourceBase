<template>
	<div class="sys-job-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="Assignment Number">
					<el-input v-model="state.queryParams.jobId" placeholder="Assignment Number" clearable />
				</el-form-item>
				<el-form-item label="Group Name">
					<el-select v-model="state.queryParams.groupName" placeholder="Group Name" clearable>
						<el-option v-for="item in state.groupsData" :key="item" :label="item" :value="item" />
					</el-select>
				</el-form-item>
				<el-form-item label="Description information">
					<el-input v-model="state.queryParams.description" placeholder="Description information" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysJob:pageJobDetail'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-tooltip content="Increase homework">
							<el-button icon="ele-CirclePlus" @click="openAddJobDetail" v-auth="'sysJob:addJobDetail'"> </el-button>
						</el-tooltip>
						<el-tooltip content="Start all jobs">
							<el-button icon="ele-VideoPlay" @click="startAllJob" />
						</el-tooltip>
						<el-tooltip content="Pause all jobs">
							<el-button icon="ele-VideoPause" @click="pauseAllJob" />
						</el-tooltip>
					</el-button-group>
					<el-button-group>
						<el-tooltip content="Force wake up the job scheduler">
							<el-button icon="ele-AlarmClock" @click="cancelSleep" />
						</el-tooltip>
						<el-tooltip content="Force the persistence of all jobs to be triggered">
							<el-button icon="ele-Connection" @click="persistAll" />
						</el-tooltip>
					</el-button-group>
					<el-button icon="ele-Coin" @click="openJobCluster" plain> Cluster control </el-button>
					<el-button icon="ele-Grid" @click="openJobDashboard" plain> task board </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.jobData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="expand" fixed>
					<template #default="scope">
						<el-table :data="(scope.row as JobDetailOutput).jobTriggers" border size="small">
							<el-table-column type="index" label="No" width="55" align="center" fixed />
							<el-table-column prop="triggerId" label="Trigger number" width="180" header-align="center" fixed show-overflow-tooltip />
							<el-table-column prop="triggerType" label="Type" width="200" header-align="center" show-overflow-tooltip />
							<!-- <el-table-column prop="assemblyName" label="assembly" show-overflow-tooltip /> -->
							<el-table-column prop="args" label="Parameter" header-align="center" show-overflow-tooltip />
							<el-table-column prop="description" label="Description" width="120" header-align="center" show-overflow-tooltip />
							<el-table-column prop="status" label="state" width="120" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag type="warning" effect="plain" v-if="(scope.row as SysJobTrigger).status == 0"> Backlog </el-tag>
									<el-tag effect="plain" v-if="(scope.row as SysJobTrigger).status == 1"> ready </el-tag>
									<el-tag type="success" effect="plain" v-if="(scope.row as SysJobTrigger).status == 2"> Running </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 3"> Pause </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 4"> blocking </el-tag>
									<el-tag effect="plain" v-if="(scope.row as SysJobTrigger).status == 5"> Transition from failure to ready </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 6"> Archive </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 7"> collapse </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 8"> Over limit </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 9"> No trigger time </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 10"> Not started </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 11"> Unknown job trigger </el-tag>
									<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 12"> Unknown job handler </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="startTime" label="start time" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="endTime" label="end time" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="lastRunTime" label="Recent run time" width="130" align="center" show-overflow-tooltip />
							<el-table-column prop="nextRunTime" label="Next Run Time" width="130" align="center" show-overflow-tooltip />
							<el-table-column prop="numberOfRuns" label="Number of triggers" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="maxNumberOfRuns" label="Maximum trigger count" width="120" align="center" show-overflow-tooltip />
							<el-table-column prop="numberOfErrors" label="Number of errors" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="maxNumberOfErrors" label="Maximum number of errors" width="120" align="center" show-overflow-tooltip />
							<el-table-column prop="numRetries" label="Number of retries" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="retryTimeout" label="Retry interval ms" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="startNow" label="Whether to start immediately" width="100" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="(scope.row as SysJobTrigger).startNow == true"> Yes </el-tag>
									<el-tag type="info" v-else> no </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="runOnStart" label="Whether to execute once at startup" width="150" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="(scope.row as SysJobTrigger).runOnStart == true"> Yes </el-tag>
									<el-tag type="info" v-else> no </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="resetOnlyOnce" label="Whether to reset the number of triggers" width="120" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="(scope.row as SysJobTrigger).resetOnlyOnce == true"> Yes </el-tag>
									<el-tag type="info" v-else> no </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="updatedTime" label="Update Time" width="130" align="center" show-overflow-tooltip />
							<el-table-column label="Operation" width="140" align="center" show-overflow-tooltip fixed="right">
								<template #default="scope">
									<el-tooltip content="Start trigger">
										<el-button size="small" type="primary" icon="ele-VideoPlay" text @click="startTrigger(scope.row)" />
									</el-tooltip>
									<el-tooltip content="Pause trigger">
										<el-button size="small" type="primary" icon="ele-VideoPause" text @click="pauseTrigger(scope.row)" />
									</el-tooltip>
									<el-tooltip content="EditTrigger">
										<el-button size="small" type="primary" icon="ele-Edit" text @click="openEditJobTrigger(scope.row)"> </el-button>
									</el-tooltip>
									<el-tooltip content="Delete trigger">
										<el-button size="small" type="danger" icon="ele-Delete" text @click="delJobTrigger(scope.row)"> </el-button>
									</el-tooltip>
								</template>
							</el-table-column>
						</el-table>
					</template>
				</el-table-column>
				<el-table-column type="index" label="No" width="55" align="center" fixed />
				<el-table-column prop="jobDetail.jobId" label="Assignment Number" width="180" header-align="center" fixed>
					<template #default="scope">
						<div style="display: flex; align-items: center">
							<el-icon><timer /></el-icon>
							<span style="margin-left: 5px">{{ (scope.row as JobDetailOutput).jobDetail?.jobId }}</span>
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="jobDetail.groupName" label="Group Name" width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="jobDetail.jobType" label="Type" width="200" header-align="center" show-overflow-tooltip />
				<!-- <el-table-column prop="jobDetail.assemblyName" label="assembly" show-overflow-tooltip /> -->
				<el-table-column prop="jobDetail.description" label="Description" header-align="center" show-overflow-tooltip />
				<el-table-column prop="jobDetail.concurrent" label="Execution method" width="90" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="(scope.row as JobDetailOutput).jobDetail?.concurrent == true"> Parallel </el-tag>
						<el-tag type="warning" v-else> serial </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="jobDetail.createType" label="Job creation type" width="110" align="center" show-overflow-tooltip>
					<template #default="scope">
						<g-sys-dict v-model="scope.row.jobDetail.createType" code="JobCreateTypeEnum" />
					</template>
				</el-table-column>
				<!-- <el-table-column prop="jobDetail.includeAnnotations" label="Scan attribute triggers" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="(scope.row as JobDetailOutput).jobDetail?.includeAnnotations == true"> Yes </el-tag>
						<el-tag v-else> no </el-tag>
					</template>
				</el-table-column> -->
				<el-table-column prop="jobDetail.updatedTime" label="Update Time" width="180" align="center" show-overflow-tooltip />
				<el-table-column prop="jobDetail.properties" label="Extra data" header-align="center" show-overflow-tooltip>
					<template #default="scope">
						<span v-if="(scope.row as JobDetailOutput).jobDetail?.createType != JobCreateTypeEnum.NUMBER_2"> {{ (scope.row as JobDetailOutput).jobDetail?.properties }} </span>
						<div v-else style="text-align: center">
							<el-popover placement="left" :width="400" trigger="hover">
								<template #reference>
									<el-tag effect="plain" type="info"> Request parameters </el-tag>
								</template>
								<el-descriptions title="HTTP request parameters" :column="1" size="small" :border="true">
									<el-descriptions-item label="Request address" label-align="right">
										{{ getHttpJobMessage((scope.row as JobDetailOutput).jobDetail?.properties).requestUri }}
									</el-descriptions-item>
									<el-descriptions-item label="Request Method" label-align="right">
										{{ getHttpMethodDesc(getHttpJobMessage((scope.row as JobDetailOutput).jobDetail?.properties).httpMethod) }}
									</el-descriptions-item>
									<el-descriptions-item label="Request message body" label-align="right">
										{{ getHttpJobMessage((scope.row as JobDetailOutput).jobDetail?.properties).body }}
									</el-descriptions-item>
								</el-descriptions>
							</el-popover>
						</div>
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="270" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tooltip content="Execution Record">
							<el-button size="small" type="primary" icon="ele-Timer" text @click="openJobTriggerRecord(scope.row)"> </el-button>
						</el-tooltip>
						<el-tooltip content="Add trigger">
							<el-button size="small" type="primary" icon="ele-CirclePlus" text @click="openAddJobTrigger(scope.row)"> </el-button>
						</el-tooltip>
						<el-tooltip content="Execute job">
							<el-button size="small" type="primary" icon="ele-CircleCheck" text @click="runJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="Start Job">
							<el-button size="small" type="primary" icon="ele-VideoPlay" text @click="startJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="Suspend operations">
							<el-button size="small" type="primary" icon="ele-VideoPause" text @click="pauseJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="Cancel job">
							<el-button size="small" type="primary" icon="ele-CircleClose" text @click="cancelJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="Edit Homework">
							<el-button size="small" type="primary" icon="ele-Edit" text @click="openEditJobDetail(scope.row)" v-auth="'sysJob:updateJobDetail'"> </el-button>
						</el-tooltip>
						<el-tooltip content="Delete homework">
							<el-button size="small" type="danger" icon="ele-Delete" text @click="delJobDetail(scope.row)" v-auth="'sysJob:deleteJobDetail'"> </el-button>
						</el-tooltip>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<el-drawer v-model="state.isVisibleDrawer" title="Job trigger run records" size="45%">
			<el-card shadow="hover" style="margin: 8px; padding-bottom: 15px; height: calc(100% - 16px); display: flex; flex-direction: column;">
				<el-table :data="state.triggerRecordData" style="height: 100%" v-loading="state.loading2" border>
					<el-table-column type="index" label="No" width="55" align="center" />
					<el-table-column prop="jobId" label="Assignment Number" min-width="120" header-align="center" show-overflow-tooltip />
					<el-table-column prop="triggerId" label="Trigger number" min-width="120" header-align="center" show-overflow-tooltip />
					<el-table-column prop="numberOfRuns" label="Current number of runs" min-width="120" align="center" show-overflow-tooltip />
					<el-table-column prop="lastRunTime" label="Recent run time" min-width="180" header-align="center" show-overflow-tooltip />
					<el-table-column prop="nextRunTime" label="Next Run Time" min-width="180" header-align="center" show-overflow-tooltip />
					<el-table-column prop="status" label="trigger state" min-width="120" align="center" show-overflow-tooltip>
						<template #default="scope">
							<el-tag type="warning" effect="plain" v-if="(scope.row as SysJobTrigger).status == 0"> Backlog </el-tag>
							<el-tag effect="plain" v-if="(scope.row as SysJobTrigger).status == 1"> ready </el-tag>
							<el-tag type="success" effect="plain" v-if="(scope.row as SysJobTrigger).status == 2"> Running </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 3"> Pause </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 4"> blocking </el-tag>
							<el-tag effect="plain" v-if="(scope.row as SysJobTrigger).status == 5"> Transition from failure to ready </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 6"> Archive </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 7"> collapse </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 8"> Over limit </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 9"> No trigger time </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 10"> Not started </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 11"> Unknown job trigger </el-tag>
							<el-tag type="danger" effect="plain" v-if="(scope.row as SysJobTrigger).status == 12"> Unknown job handler </el-tag>
						</template>
					</el-table-column>
					<el-table-column prop="result" label="Execution result" min-width="100" header-align="center" show-overflow-tooltip />
					<el-table-column prop="elapsedTime" label="Time consuming" min-width="80" align="center" show-overflow-tooltip />
					<el-table-column prop="createdTime" label="Creation Time" min-width="180" align="center" show-overflow-tooltip />
				</el-table>
				<el-pagination
					v-model:currentPage="state.tableParams2.page"
					v-model:page-size="state.tableParams2.pageSize"
					:total="state.tableParams2.total"
					:page-sizes="[10, 20, 50, 100]"
					size="small"
					background
					@size-change="handleSizeChange2"
					@current-change="handleCurrentChange2"
					layout="total, sizes, prev, pager, next, jumper"
				/>
			</el-card>
		</el-drawer>

		<EditJobDetail ref="editJobDetailRef" :title="state.editJobDetailTitle" @handleQuery="handleQuery" />
		<EditJobTrigger ref="editJobTriggerRef" :title="state.editJobTriggerTitle" @handleQuery="handleQuery" />
		<JobCluster ref="editJobClusterRef" />
	</div>
</template>

<script lang="ts" setup name="sysJob">
import { nextTick, onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import { Timer } from '@element-plus/icons-vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { JobCreateTypeEnum, JobDetailOutput, SysJobTrigger } from '/@/api-services/models';
import EditJobDetail from '/@/views/system/job/component/editJobDetail.vue';
import EditJobTrigger from '/@/views/system/job/component/editJobTrigger.vue';
import JobCluster from '/@/views/system/job/component/jobCluster.vue';

const router = useRouter();
const editJobDetailRef = ref<InstanceType<typeof EditJobDetail>>();
const editJobTriggerRef = ref<InstanceType<typeof EditJobTrigger>>();
const editJobClusterRef = ref<InstanceType<typeof JobCluster>>();
const state = reactive({
	loading: false,
	jobData: [] as Array<JobDetailOutput>,
	queryParams: {
		jobId: undefined,
		groupName: undefined,
		description: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	tableParams2: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editJobDetailTitle: '',
	editJobTriggerTitle: '',
	loading2: false,
	isVisibleDrawer: false,
	triggerRecordData: [] as any,
	currentJob: {} as any,
	groupsData: [] as Array<string>,
});

onMounted(async () => {
	await handleQuery();
	// Get drop-down collection of group names
	nextTick(async () => {
		const { data } = await getAPI(SysJobApi).apiSysJobListJobGroupPost();
		state.groupsData = data.result ?? [];
	});
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysJobApi).apiSysJobPageJobDetailPost(params);
	state.jobData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = async () => {
	state.queryParams.jobId = undefined;
	state.queryParams.groupName = undefined;
	state.queryParams.description = undefined;
	await handleQuery();
};

// Open the new job page
const openAddJobDetail = () => {
	state.editJobDetailTitle = 'Add Homework';
	editJobDetailRef.value?.openDialog({ concurrent: true, includeAnnotations: true, groupName: 'default', createType: JobCreateTypeEnum.NUMBER_2 });
};

// Open the edit job page
const openEditJobDetail = (row: JobDetailOutput) => {
	state.editJobDetailTitle = 'Edit Homework';
	editJobDetailRef.value?.openDialog(row.jobDetail);
};

// Delete job
const delJobDetail = (row: JobDetailOutput) => {
	ElMessageBox.confirm(`Are you sure you want to delete the job: 【${row.jobDetail?.jobId}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysJobApi).apiSysJobDeleteJobDetailPost({ jobId: row.jobDetail?.jobId });
			await handleQuery();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Open the new trigger page
const openAddJobTrigger = (row: JobDetailOutput) => {
	state.editJobTriggerTitle = 'Add trigger';
	editJobTriggerRef.value?.openDialog({
		jobId: row.jobDetail?.jobId,
		retryTimeout: 1000,
		startNow: true,
		runOnStart: true,
		resetOnlyOnce: true,
		triggerType: 'Furion.Schedule.PeriodTrigger',
	});
};

// Open the edit trigger page
const openEditJobTrigger = (row: SysJobTrigger) => {
	state.editJobTriggerTitle = 'EditTrigger';
	editJobTriggerRef.value?.openDialog(row);
};

// delete trigger
const delJobTrigger = (row: SysJobTrigger) => {
	ElMessageBox.confirm(`Are you sure to delete the trigger: [${row.triggerId}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysJobApi).apiSysJobDeleteJobTriggerPost({ jobId: row.jobId, triggerId: row.triggerId });
			await handleQuery();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Change page capacity
const handleSizeChange = async (val: number) => {
	state.tableParams.pageSize = val;
	await handleQuery();
};

// Change page number
const handleCurrentChange = async (val: number) => {
	state.tableParams.page = val;
	await handleQuery();
};

// Start all jobs
const startAllJob = async () => {
	await getAPI(SysJobApi).apiSysJobStartAllJobPost();
	ElMessage.success('Start all jobs');
};

// Pause all jobs
const pauseAllJob = async () => {
	await getAPI(SysJobApi).apiSysJobPauseAllJobPost();
	ElMessage.success('Pause all jobs');
};

// Execute a job
const runJob = async (row: JobDetailOutput) => {
	await getAPI(SysJobApi).apiSysJobRunJobPost({ jobId: row.jobDetail?.jobId });
	ElMessage.success('Execute job');
};

// start a job
const startJob = async (row: JobDetailOutput) => {
	await getAPI(SysJobApi).apiSysJobStartJobPost({ jobId: row.jobDetail?.jobId });
	ElMessage.success('Start Job');
};

// pause a job
const pauseJob = async (row: JobDetailOutput) => {
	await getAPI(SysJobApi).apiSysJobPauseJobPost({ jobId: row.jobDetail?.jobId });
	ElMessage.success('Suspend operations');
};

// Cancel a job
const cancelJob = async (row: JobDetailOutput) => {
	await getAPI(SysJobApi).apiSysJobCancelJobPost({ jobId: row.jobDetail?.jobId });
	ElMessage.success('Cancel job');
};

// start trigger
const startTrigger = async (row: SysJobTrigger) => {
	await getAPI(SysJobApi).apiSysJobStartTriggerPost({ jobId: row.jobId, triggerId: row.triggerId });
	ElMessage.success('Start trigger');
};

// pause trigger
const pauseTrigger = async (row: SysJobTrigger) => {
	await getAPI(SysJobApi).apiSysJobPauseTriggerPost({ jobId: row.jobId, triggerId: row.triggerId });
	ElMessage.success('Pause trigger');
};

// Force wake up job scheduler
const cancelSleep = async () => {
	await getAPI(SysJobApi).apiSysJobCancelSleepPost();
	ElMessage.success('Force wake up the job scheduler');
};

// Force the persistence of all jobs to be triggered
const persistAll = async () => {
	await getAPI(SysJobApi).apiSysJobPersistAllPost();
	ElMessage.success('Force the persistence of all jobs to be triggered');
};

// Open the cluster control page
const openJobCluster = () => {
	editJobClusterRef.value?.openDrawer();
};

// Open task board
const openJobDashboard = () => {
	router.push({
		path: '/platform/job/dashboard',
	});
};

// Get HttpJobMessage based on task properties
const getHttpJobMessage = (properties: string | undefined | null): HttpJobMessage => {
	if (properties === undefined || properties === null || properties === '') return {};

	const propData = JSON.parse(properties);
	const httpJobMessageNet = JSON.parse(propData['HttpJob']); // HttpJobMessage with backend capitalized

	return {
		requestUri: httpJobMessageNet.RequestUri,
		httpMethod: JSON.stringify(httpJobMessageNet.HttpMethod),
		body: httpJobMessageNet.Body,
	};
};

// Get the corresponding description of the request method
const getHttpMethodDesc = (httpMethodStr: string | undefined | null): string => {
	if (httpMethodStr === undefined || httpMethodStr === null || httpMethodStr === '') return '';

	for (const key in editJobDetailRef.value?.httpMethodDef) {
		if (editJobDetailRef.value?.httpMethodDef[key] === httpMethodStr) return key;
	}
	return '';
};

// Turn on job trigger run logging
const openJobTriggerRecord = async (row: any) => {
	state.currentJob = row;
	state.isVisibleDrawer = true;
	await handleQuery2();
};

// Job trigger runs record query operation
const handleQuery2 = async () => {
	state.loading2 = true;
	let params = Object.assign({ jobId: state.currentJob.jobDetail.jobId }, state.tableParams2); //state.currentJob.jobTriggers[0].triggerId
	var res = await getAPI(SysJobApi).apiSysJobPageJobTriggerRecordPost(params);
	state.triggerRecordData = res.data.result?.items ?? [];
	state.tableParams2.total = res.data.result?.total;
	state.loading2 = false;
};

// Job trigger running record-change page capacity
const handleSizeChange2 = async (val: number) => {
	state.tableParams2.pageSize = val;
	await handleQuery2();
};

// Job trigger running record-change page number
const handleCurrentChange2 = async (val: number) => {
	state.tableParams2.page = val;
	await handleQuery2();
};
</script>

<style lang="scss" scoped>
:deep(.el-form-item__content) {
    gap: 12px;

    .el-button+.el-button {
        margin-left: 0px;
    }
}

:deep(.el-table__expanded-cell) {
    padding: 10px 45px !important;
}

:deep(.el-descriptions__body) {
    .el-descriptions__table { table-layout: fixed; }
    .el-descriptions__label { width: 80px; }
}
</style>
