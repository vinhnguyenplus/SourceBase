<template>
	<div class="sys-cache-container h100">
		<el-splitter class="smallbar-el-splitter">
			<el-splitter-panel size="20%" :min="200">
				<CardPro title="Cache List" v-loading="state.loading" full-height body-style="overflow:auto">
					<template #suffix>
						<el-button icon="ele-Refresh" type="success" circle plain @click="handleQuery" v-auth="'sysCache:keyList'" />
                        <el-button icon="ele-DeleteFilled" type="danger" circle plain @click="clearCache" v-auth="'sysCache:clear'"> </el-button>
					</template>
					<el-tree
						ref="treeRef"
						class="filter-tree"
						:data="state.cacheData"
						node-key="id"
						:props="{ children: 'children', label: 'name' }"
						@node-click="nodeClick"
						:default-checked-keys="state.cacheData"
						highlight-current
						check-strictly
						default-expand-all
						accordion
					/>
				</CardPro>
			</el-splitter-panel>
			<el-splitter-panel :min="200">
				<CardPro :title="`Cache data ${state.cacheKey ? `[${state.cacheKey}]` : ''}`" v-loading="state.loading1" full-height body-style="overflow:auto">
                    <template #suffix>
                        <el-button icon="ele-Delete" type="danger" @click="delCache" v-auth="'sysCache:delete'"> Clear cache </el-button>
                    </template>
                    <vue-json-pretty :data="state.cacheValue" showLength showIcon showLineNumber showSelectController />
				</CardPro>
			</el-splitter-panel>
		</el-splitter>
	</div>
</template>

<script lang="ts" setup name="sysCache">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, ElTree } from 'element-plus';
import CardPro from '/@/components/CardPro/index.vue';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';
import 'splitpanes/dist/splitpanes.css';

import { getAPI } from '/@/utils/axios-utils';
import { SysCacheApi } from '/@/api-services';

const treeRef = ref<InstanceType<typeof ElTree>>();
const currentNode = ref<any>({});
const state = reactive({
	loading: false,
	loading1: false,
	cacheData: [] as any,
	cacheValue: undefined as any,
	cacheKey: undefined,
});

onMounted(async () => {
	await handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.cacheData = [];
	state.cacheValue = undefined;
	state.cacheKey = undefined;

	state.loading = true;
	var res = await getAPI(SysCacheApi).apiSysCacheKeyListGet();
	let keyList: any = res.data.result;

	// Construction tree (separated by semicolon)
	for (let i = 0; i < keyList.length; i++) {
		let keyNames = keyList[i].split(':');
		let pName = keyNames[0];
		if (state.cacheData.filter((x: any) => x.name == pName).length === 0) {
			state.cacheData.push({
				id: keyNames.length > 1 ? 0 : keyList[i],
				name: pName,
				children: [],
			});
		}
		if (keyNames.length > 1) {
			let pNode = state.cacheData.filter((x: any) => x.name == pName)[0] || {};
			pNode.children.push({
				id: keyList[i],
				name: keyList[i].substr(pName.length + 1),
			});
		}
	}
	state.loading = false;
};

// delete
const delCache = () => {
	if (currentNode.value.id == 0) {
		ElMessage.warning('Deleting the top-level cache is prohibited');
		return;
	}
	ElMessageBox.confirm(`Are you sure you want to delete the cache: 【${currentNode.value.id}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysCacheApi).apiSysCacheDeleteKeyPost(currentNode.value.id);
			await handleQuery();
			state.cacheValue = undefined;
			state.cacheKey = undefined;
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Clear
const clearCache = () => {
	ElMessageBox.confirm(`Are you sure you want to clear all caches?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysCacheApi).apiSysCacheClearPost();
			await handleQuery();
			state.cacheValue = undefined;
			state.cacheKey = undefined;
			ElMessage.success('Clearsuccess');
		})
		.catch(() => {});
};

// tree click
const nodeClick = async (node: any) => {
	if (node.id == 0) return;

	currentNode.value = node;
	state.loading1 = true;
	var res = await getAPI(SysCacheApi).apiSysCacheValueKeyGet(node.id);
	// state.cacheValue = JSON.parse(res.data.result);
	var result = res.data.result;
	if (typeof result == 'string') {
		try {
			var obj = JSON.parse(result);
			if (typeof obj == 'object') {
				state.cacheValue = obj;
			} else {
				state.cacheValue = result;
			}
		} catch (e) {
			state.cacheValue = result;
		}
	} else {
		state.cacheValue = result;
	}

	state.cacheKey = node.id;
	state.loading1 = false;
};
</script>
