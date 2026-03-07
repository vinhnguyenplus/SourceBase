<template>
	<el-card class="box-card" shadow="hover" style="height: 100%" body-style="height:calc(100% - 65px); overflow:auto">
		<template #header>
			<div class="card-header">
				<div class="tree-h-flex">
					<div class="tree-h-left">
						<el-input :prefix-icon="Search" v-model="filterText" placeholder="Administrative region name" />
					</div>
					<div class="tree-h-right">
						<el-dropdown @command="handleCommand">
							<el-button style="margin-left: 8px; width: 34px">
								<el-icon class="el-icon--center">
									<more-filled />
								</el-icon>
							</el-button>
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item command="expandAll">Expand all</el-dropdown-item>
									<el-dropdown-item command="collapseAll">Collapse all</el-dropdown-item>
									<el-dropdown-item command="rootNode">root node</el-dropdown-item>
									<el-dropdown-item command="refresh">Refresh</el-dropdown-item>
								</el-dropdown-menu>
							</template>
						</el-dropdown>
					</div>
				</div>
			</div>
		</template>
		<div v-loading="state.loading">
			<el-tree
				ref="treeRef"
				class="filter-tree"
				:data="state.regionData"
				node-key="id"
				:props="{ children: 'children', label: 'name' }"
				:filter-node-method="filterNode"
				@node-click="nodeClick"
				highlight-current
				check-strictly
				accordion
				lazy
				:load="loadNode"
			/>
		</div>
	</el-card>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref, watch } from 'vue';
import type { ElTree } from 'element-plus';
import { Search, MoreFilled } from '@element-plus/icons-vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRegionApi } from '/@/api-services/api';
import { SysRegion } from '/@/api-services/models';

const filterText = ref('');
const treeRef = ref<InstanceType<typeof ElTree>>();
const state = reactive({
	loading: false,
	regionData: [] as Array<SysRegion>,
});

onMounted(() => {
	initTreeData();
});

watch(filterText, (val) => {
	treeRef.value!.filter(val);
});

const initTreeData = async () => {
	state.loading = true;
	var res = await getAPI(SysRegionApi).apiSysRegionListGet(0);
	state.regionData = res.data.result ?? [];
	state.loading = false;
};

const loadNode = async (node: any, resolve: any) => {
	if (node.data == undefined || Array.isArray(node.data)) return;

	state.loading = true;
	var res = await getAPI(SysRegionApi).apiSysRegionListGet(node.data.id);
	var data = res.data.result ?? [];
	state.loading = false;
	resolve(data);
};

/**
 * ObtainSelectinof the nodeKeys
 * @returns Selectinof the nodeKeynumbergroup
 */
const getCheckedKeys = () => {
    return treeRef.value!.getCheckedKeys();
};

/**
 * ObtainCurrently selectedinNode
 * @returns Currently selectedinNode
 */
const getCurrentNode = () => {
    return treeRef.value!.getCurrentNode();
};

/**
 * ObtainCurrently selectedinNumber of paths of the nodegroup
 * （fromroot nodeTo the current node，Arrange in index order）
 * @returns {Array<{ id: number, name: string }>} Number of pathsgroup
 */
const getCurrentPath = () => {
    const currentNode = getCurrentNode();
    if(!currentNode) return null;

    const cascaderData = getCascaderData(currentNode);
    const path = [] as Array<{ id: number, name: string }>;
    let node = cascaderData;
    while(node) {
        path.push({ id: node.id, name: node.name });
        node = node.child;
    }
    return path;
};

// Recursively obtain the currently selected cascade data
const getCascaderData = (child: any) => {
    const parent = treeRef.value!.getNode(child.pid)?.data as SysRegion & { child?: SysRegion };
    if(!parent) return child;

    parent.child = child;
    if(parent.pid != 0) {
        return getCascaderData(parent);
    }

    return parent;
};

const filterNode = (value: string, data: any) => {
	if (!value) return true;
	return data.name.includes(value);
};

const handleCommand = async (command: string | number | object) => {
	if ('expandAll' == command) {
		for (let i = 0; i < treeRef.value!.store._getAllNodes().length; i++) {
			treeRef.value!.store._getAllNodes()[i].expanded = true;
		}
	} else if ('collapseAll' == command) {
		for (let i = 0; i < treeRef.value!.store._getAllNodes().length; i++) {
			treeRef.value!.store._getAllNodes()[i].expanded = false;
		}
	} else if ('refresh' == command) {
		initTreeData();
	} else if ('rootNode' == command) {
		emits('node-click', { id: 0, name: '' });
	}
};

// Interaction logic with parent component
const emits = defineEmits(['node-click']);
const nodeClick = (node: any) => {
	emits('node-click', { id: node.id, name: node.name });
};

// Export object
defineExpose({ initTreeData, getCheckedKeys, getCurrentNode, getCurrentPath });
</script>

<style lang="scss" scoped>
.tree-h-flex {
	display: flex;
}

.tree-h-left {
	flex: 1;
	width: 100%;
}

.tree-h-right {
	width: 42px;
	min-width: 42px;
}
</style>
