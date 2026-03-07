<template>
	<div class="sys-stress-test">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-DataLine /> </el-icon>
					<span> Interface Stress Test Parameters </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" v-loading="state.loading">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Request Method" :rules="[{ required: true, message: 'Request method cannot be empty', trigger: 'blur' } ]">
							<el-select v-model="state.ruleForm.requestMethod" placeholder="Request Method">
								<el-option :value="'GET'">GET</el-option>
								<el-option :value="'PUT'">PUT</el-option>
								<el-option :value="'POST'">POST</el-option>
								<el-option :value="'DELETE'">DELETE</el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Request address" :rules="[{ required: true, message: 'Request address cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.requestUri" placeholder="Request address" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Number of rounds" :rules="[{ required: true, message: 'Number of rounds cannot be empty', trigger: 'blur' }]">
							<el-input-number v-model="state.ruleForm.numberOfRounds" placeholder="Number of rounds" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Number of requests per round" :rules="[{ required: true, message: 'The number of requests per round cannot be empty', trigger: 'blur' }]">
							<el-input-number v-model="state.ruleForm.numberOfRequests" :step="100" placeholder="Number of requests per round" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Maximum concurrency">
							<el-input-number v-model="state.ruleForm.maxDegreeOfParallelism" :step="5" placeholder="Maximum concurrency" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-tabs v-model="state.activeName" addable @tab-add="addParams()">
							<el-tab-pane label="Headers" name="1">
								<template v-for="(item, index) in state.ruleForm.headers" :key="index">
                                    <div class="flex flex-items-center">
                                        <div class="flex-auto">
                                            <el-row :gutter="15" class="w100">
                                                <el-col :xs="24" :sm="7" :md="7" :lg="7" :xl="7" class="mt5 mb5">
                                                    <el-input v-model="item[0]" placeholder="Parameter Name" clearable />
                                                </el-col>
                                                <el-col :xs="24" :sm="17" :md="17" :lg="17" :xl="17" class="mt5 mb5">
                                                    <el-input v-model="item[1]" placeholder="Parameter value" clearable />
                                                </el-col>
                                            </el-row>
                                        </div>
                                        <div style="width: 40px;">
                                            <el-button type="danger" size="small" icon="ele-Delete" text @click="() => state.ruleForm.headers.splice(index,1)" />
                                        </div>
                                    </div>
                                </template>
							</el-tab-pane>
							<el-tab-pane label="Body" name="2">
                                <template v-for="(item, index) in state.ruleForm.requestParameters" :key="index">
                                    <div class="flex flex-items-center">
                                        <div class="flex-auto">
                                            <el-row :gutter="15" class="w100">
                                                <el-col :xs="24" :sm="7" :md="7" :lg="7" :xl="7" class="mt5 mb5">
                                                    <el-input v-model="item[0]" placeholder="Parameter Name" clearable />
                                                </el-col>
                                                <el-col :xs="24" :sm="17" :md="17" :lg="17" :xl="17" class="mt5 mb5">
                                                    <el-input v-model="item[1]" placeholder="Parameter value" clearable />
                                                </el-col>
                                            </el-row>
                                        </div>
                                        <div style="width: 40px;">
                                            <el-button type="danger" size="small" icon="ele-Delete" text @click="() => state.ruleForm.requestParameters.splice(index,1)" />
                                        </div>
                                    </div>
                                </template>
							</el-tab-pane>
							<el-tab-pane label="Path" name="3">
                                <template v-for="(item, index) in state.ruleForm.pathParameters" :key="index">
                                    <div class="flex flex-items-center">
                                        <div class="flex-auto">
                                            <el-row :gutter="15" class="w100">
                                                <el-col :xs="24" :sm="7" :md="7" :lg="7" :xl="7" class="mt5 mb5">
                                                    <el-input v-model="item[0]" placeholder="Parameter Name" clearable />
                                                </el-col>
                                                <el-col :xs="24" :sm="17" :md="17" :lg="17" :xl="17" class="mt5 mb5">
                                                    <el-input v-model="item[1]" placeholder="Parameter value" clearable/>
                                                </el-col>
                                            </el-row>
                                        </div>
                                        <div style="width: 40px;">
                                            <el-button type="danger" size="small" icon="ele-Delete" text @click="() => state.ruleForm.pathParameters.splice(index,1)" />
                                        </div>
                                    </div>
                                </template>
							</el-tab-pane>
							<el-tab-pane label="Query" name="4">
                                <template v-for="(item, index) in state.ruleForm.queryParameters" :key="index">
                                    <div class="flex flex-items-center">
                                        <div class="flex-auto">
                                            <el-row :gutter="15" class="w100">
                                                <el-col :xs="24" :sm="7" :md="7" :lg="7" :xl="7" class="mt5 mb5">
                                                    <el-input v-model="item[0]" placeholder="Parameter Name" clearable/>
                                                </el-col>
                                                <el-col :xs="24" :sm="17" :md="17" :lg="17" :xl="17" class="mt5 mb5">
                                                    <el-input v-model="item[1]" placeholder="Parameter value" clearable/>
                                                </el-col>
                                            </el-row>
                                        </div>
                                        <div style="width: 40px;">
                                            <el-button type="danger" size="small" icon="ele-Delete" text @click="() => state.ruleForm.queryParameters.splice(index,1)" />
                                        </div>
                                    </div>
                                </template>
							</el-tab-pane>
						</el-tabs>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer" v-loading="state.loading">
					<el-button @click="() => state.isShowDialog = false">Cancel</el-button>
					<el-button type="primary" @click="submit" v-reclick="1000">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysStressTest">
import { reactive, ref } from 'vue';
import {SysCommonApi} from "/@/api-services";
import {getAPI} from "/@/utils/axios-utils";

const emits = defineEmits(['refreshData']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	activeName: '1',
	loading: false,
	ruleForm: {
		requestUri: '',
		requestMethod: 'GET',
		numberOfRounds: 1,
		numberOfRequests: 100,
		maxDegreeOfParallelism: 200,
		requestParameters: [[]] as [[]] | {},
		queryParameters: [[]] as [[]] | {},
		pathParameters: [[]] as [[]] | {},
		headers: [[]] as [[]] | {},
	},
});

// Format parameters
const formatParameter = (params: any[] | {}) => {
	if (Array.isArray(params)) {
		return Object.fromEntries(params.filter(e => e.length === 2));
	} else if (typeof params === 'object' && params !== null) {
		return Object.entries(params);
	}
	return {};
};

// Open pop-up window
const openDialog = (row: any) => {
	const newRow = { ...state.ruleForm, ...row }; // Merge default and new values
	state.ruleForm = {
		...newRow,
		requestMethod: row.requestMethod?.toUpperCase() ?? 'GET',
	};
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		try {
			state.loading = true;

			// Create a new object to hold the formatted data
			const formattedRuleForm = {
				...state.ruleForm,
				headers: formatParameter(state.ruleForm.headers),
				pathParameters: formatParameter(state.ruleForm.pathParameters),
				queryParameters: formatParameter(state.ruleForm.queryParameters),
				requestParameters: formatParameter(state.ruleForm.requestParameters),
			};

			// Ensure that all parameters that may be empty objects are correctly set to undefined
			['headers', 'pathParameters', 'queryParameters', 'requestParameters'].forEach(paramKey => {
				if (Object.keys(formattedRuleForm[paramKey] || {}).length === 0) {
					formattedRuleForm[paramKey] = undefined;
				}
			});

			emits('refreshData', await getAPI(SysCommonApi).apiSysCommonStressTestPost(formattedRuleForm,{ timeout: 0 }).then(res => res.data.result));
			state.isShowDialog = false;
		} finally {
			state.loading = false;
		}
	});
};

// Add parameters
const addParams = () => {
	const paramType = ['headers', 'requestParameters', 'pathParameters', 'queryParameters'][+state.activeName - 1];
	if (Array.isArray(state.ruleForm[paramType])) {
		state.ruleForm[paramType].push([null, null]);
	} else if (typeof state.ruleForm[paramType] === 'object') {
		state.ruleForm[paramType] = [[null, null]];
	}
};

// Export object
defineExpose({ openDialog });
</script>
