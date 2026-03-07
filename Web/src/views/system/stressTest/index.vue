<template>
	<div class="sys-stress-test h100 overlay-none">
		<el-splitter class="smallbar-el-splitter overlay-hidden">
			<el-splitter-panel size="25%" :min="300">
				<CardPro title="interface list" full-height shadow="hover" v-loading="state.loading" body-style="display: flex; flex-direction: column;">
					<el-select v-model="state.swaggerUrl" @change="queryTreeNode" placeholder="InterfaceGroup" class="mb10">
						<el-option :label="item.name" :value="item.url" v-for="(item, index) in state.groupList" :key="index" />
					</el-select>
                    <div class="mb10" style="display: flex;">
                        <el-input v-model="state.keywords" placeholder="Keywords" clearable style="flex: 1; margin-right: 10px;" />
                        <el-button icon="ele-Search" v-reclick="1000" @click="queryTreeNode()" />
                    </div>
                    <el-tree ref="treeRef" class="filter-tree overlay-y"
                        :data="state.data"
                        :props="{ children: 'children', label: 'summary' }"
                        :filter-node-method="filterNode"
                        node-key="id"
                        highlight-current
                        check-strictly
                        style="flex: 1 1 0;"
                    >
                        <template #default="{ node }">
                            {{ node.label }}
                            <span class="node-button" v-if="!node.data.children">
                                <el-button size="small" icon="ele-DataLine" @click="treeNodeTest(node.data)" />
                            </span>
                        </template>
                    </el-tree>
					
				</CardPro>
			</el-splitter-panel>
			<el-splitter-panel :min="200">
				<CardPro title="Cached data" full-height shadow="hover"v-loading="state.loading">
					<template #suffix>
                        <div style="display: flex; width: 100%;">
                            <el-button type="primary" @click="showDialog(undefined)">Start testing</el-button>
                        </div>
					</template>
					<el-descriptions title="Pressure measurement parameters" label-width="180px" :column="2" class="mb20" border>
						<el-descriptions-item label="Request Method" label-align="left" align="left">
							{{ state.ruleForm.requestMethod?.toUpperCase() }}
						</el-descriptions-item>
						<el-descriptions-item label="Request address" label-align="left" align="left">
							{{ state.ruleForm.requestUri }}
						</el-descriptions-item>
						<el-descriptions-item label="Number of rounds" label-align="left" align="left">
							{{ state.ruleForm.numberOfRounds ?? 0 }}
						</el-descriptions-item>
						<el-descriptions-item label="Number of requests per round" label-align="left" align="left">
							{{ state.ruleForm.numberOfRequests ?? 0 }}
						</el-descriptions-item>
						<el-descriptions-item label="Maximum concurrency" label-align="left" align="left">
							{{ state.ruleForm.maxDegreeOfParallelism ?? 0 }}
						</el-descriptions-item>
					</el-descriptions>
					<el-descriptions title="Pressure test results" label-width="180px" :column="3" border>
						<el-descriptions-item label="Total Time (seconds)" label-align="left" align="left">
							{{ (state.result.totalTimeInSeconds ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="Number of successful requests" label-align="left" align="left">
							{{ state.result.successfulRequests ?? 0 }}
						</el-descriptions-item>
						<el-descriptions-item label="Number of failed requests" label-align="left" align="left">
							{{ state.result.failedRequests ?? 0 }}
						</el-descriptions-item>
						<el-descriptions-item label="Query rate per second (QPS)" label-align="left" align="left">
							{{ (state.result.queriesPerSecond ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="Minimum response time (milliseconds)" label-align="left" align="left">
							{{ (state.result.minResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="Maximum response time (milliseconds)" label-align="left" align="left">
							{{ (state.result.maxResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="Average response time (milliseconds)" span="3" label-align="left" align="left">
							{{ (state.result.averageResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P10 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile10ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P25 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile25ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P50 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile50ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P75 responsetimespace（millimetersecond）" label-align="left" align="left">
							{{ (state.result.percentile75ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P90 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile90ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P99 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile99ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
						<el-descriptions-item label="P999 Response Time (ms)" label-align="left" align="left">
							{{ (state.result.percentile999ResponseTime ?? 0).toFixed(2) }}
						</el-descriptions-item>
					</el-descriptions>
				</CardPro>
			</el-splitter-panel>
		</el-splitter>
		<EditStressTest ref="editStressTestRef" @refreshData="refreshData" />
	</div>
</template>

<script lang="ts" setup name="sysStressTest">
import { onMounted, reactive, ref } from 'vue';
import EditStressTest from './component/editStressTest.vue';
import CardPro from '/@/components/CardPro/index.vue';
import request, { getToken } from '/@/utils/request';
import { StressTestOutput } from "/@/api-services";
import { ElTree } from 'element-plus';
import 'splitpanes/dist/splitpanes.css';
import 'vue-json-pretty/lib/styles.css';

const editStressTestRef = ref();
const treeRef = ref<InstanceType<typeof ElTree>>();
const state = reactive({
	loading: false,
	activeName: '',
	ruleForm: {
		requestUri: '',
		requestMethod: 'GET',
		numberOfRounds: 1,
		numberOfRequests: 100,
		maxDegreeOfParallelism: 200,
		requestParameters: [[]],
		queryParameters: [[]],
		pathParameters: [[]],
		headers: [[]] as Array<Array<any>>,
	},
	keywords: undefined,
	swaggerUrl: '/swagger/Default/swagger.json',
	result: {} as StressTestOutput,
	data: [] as Array<any>,
	groupList: [] as Array<any>,
});

onMounted(async () => {
	state.groupList = await getGroupList();
	state.data = await getApiList();
});

// Get group list
const getGroupList = async () => {
	try {
		const response = await request('/swagger-resources', { method: 'get' });
        return response.data
        .filter((resource: { name: string; url: string }) => !resource.url.toLowerCase().includes('all%20groups'))
        .map((resource: { name: string; url: string }) => {
            const rawUrl = resource.url || '';
            let fixedUrl = rawUrl.startsWith('//') ? 
                rawUrl.substring(1) : 
                rawUrl;
            if (!fixedUrl.startsWith('/') && !fixedUrl.includes('://')) {
                fixedUrl = '/' + fixedUrl;
            }            
            return {
                name: decodeURIComponent(resource.name || ''),
                url: fixedUrl
            };
        });
	} catch {
		return [];
	}
}

// Interface tree node button event
const treeNodeTest = async (node: any) => {
	if (node.id == 0) return;
	state.ruleForm = {
		requestUri: location.origin + node.path,
		requestMethod: node.method,
		numberOfRounds: 1,
		numberOfRequests: 100,
		maxDegreeOfParallelism: 200,
		requestParameters: [],
		queryParameters: [],
		pathParameters: [],
		headers: [
			['Authorization', 'Bearer ' + getToken()]
		],
	};
	showDialog(state.ruleForm)
};

const showDialog = async (row: any) => {
	const newRow = row ?? { ...state.ruleForm };
	const convertToKeyValuePairs = (params: any) => {
		if (Array.isArray(params) && params.every(item => Array.isArray(item) && item.length === 2)) {
			return params
		} else if (typeof params === 'object' && params !== null) {
			return Object.entries(params)
		}
		return []
	}

	state.ruleForm = {
		...newRow,
		requestParameters: convertToKeyValuePairs(newRow.requestParameters),
		queryParameters: convertToKeyValuePairs(newRow.queryParameters),
		pathParameters: convertToKeyValuePairs(newRow.pathParameters),
		headers: convertToKeyValuePairs(newRow.headers)
	}
	editStressTestRef.value.openDialog(state.ruleForm)
}

// Refresh data
const refreshData = (data: StressTestOutput) => {
	state.result = data;
}

const getApiList = (keywords?: string | undefined) => {
	const emojiPattern = /[\u{2139}\u{2B05}-\u{2B07}\u{1F600}-\u{1F64F}\u{1F300}-\u{1F5FF}\u{1F680}-\u{1F6FF}\u{1F700}-\u{1F77F}\u{1F780}-\u{1F7FF}\u{1F800}-\u{1F8FF}\u{1F900}-\u{1F9FF}\u{1FA00}-\u{1FA6F}\u{1FA70}-\u{1FAFF}\u{2600}-\u{26FF}\u{2700}-\u{27BF}]/gu;
	return request(state.swaggerUrl, { method: 'get' }).then(({ data }) => {
		const pathMap = data.paths;
		const result = data.tags.map((e: any) => ({ path: e.name, summary: e.description?.replaceAll(emojiPattern, '') || e.name, children: [] }));
		Object.keys(pathMap).map((path) => {
			const method = Object.keys(pathMap[path])[0];
			const apiInfo = pathMap[path][method];
			if (keywords && apiInfo.summary?.indexOf(keywords) === -1) return;
			result
				.find((u: any) => u.path === apiInfo.tags[0])
				.children.push({
					path: path,
					method: method,
					summary: apiInfo.summary?.replaceAll(emojiPattern, '') ?? path,
					parameters: apiInfo.parameters,
					requestBody: apiInfo.requestBody,
					data: apiInfo,
				});
		});
		return result.filter((u: any) => u.children.length > 0);
	});
};

// Query tree nodes
const queryTreeNode = async () => {
	state.data = await getApiList(state.keywords);
}

const filterNode = (value: string, data: any) => {
	if (!value) return true;
	return data.name.includes(value);
};
</script>

<style lang="scss" scoped>
:deep(.el-collapse-item) {
	.el-collapse-item__arrow {
		float: right;
	}
}
.node-button {
	position: absolute;
	scale: 0.7;
	right: 0;
}
</style>
