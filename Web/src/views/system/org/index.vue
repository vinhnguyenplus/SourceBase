<template>
    <div class="sys-org-container">
        <el-splitter class="smallbar-el-splitter">
            <el-splitter-panel size="20%" :min="200">
                <OrgTree ref="orgTreeRef" @node-click="nodeClick" />
            </el-splitter-panel>
            <el-splitter-panel :min="200" style="overflow: auto; display: flex; flex-direction: column;">
                <el-card shadow="hover" :body-style="{ padding: 5 }">
                    <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                        <el-form-item label="Organization name">
                            <el-input v-model="state.queryParams.name" placeholder="Organization name" clearable />
                        </el-form-item>
                        <!-- <el-form-item label="Organization Code">
							<el-input v-model="state.queryParams.code" placeholder="Institution code" clearable />
						</el-form-item> -->
                        <el-form-item label="Institution type">
                            <g-sys-dict v-model="state.queryParams.type" code="org_type" render-as="select" filterable clearable />
                        </el-form-item>
                        <el-form-item>
                            <el-button-group>
                                <el-button type="primary" icon="ele-Search" @click="handleQuery"> Query </el-button>
                                <el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
                            </el-button-group>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Plus" @click="openAddOrg" v-auth="'sysOrg:add'"> Add New
                            </el-button>
                        </el-form-item>
                    </el-form>
                </el-card>

                <el-card class="full-table" shadow="hover" style="margin-top: 5px">
                    <el-table :data="state.orgData" style="width: 100%" v-loading="state.loading" row-key="id"
                        default-expand-all :tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
                        <el-table-column prop="name" label="Organization name" min-width="160" header-align="center"
                            show-overflow-tooltip />
                        <el-table-column prop="code" label="Institution code" align="center" show-overflow-tooltip />
                        <el-table-column prop="level" label="level" width="70" align="center" show-overflow-tooltip />
                        <el-table-column prop="type" label="Institution type" align="center" show-overflow-tooltip>
                            <template #default="scope">
                                <g-sys-dict v-model="scope.row.type" code="org_type" />
                            </template>
                        </el-table-column>
                        <el-table-column prop="orderNo" label="Sort" width="70" align="center" show-overflow-tooltip />
                        <el-table-column label="state" width="70" align="center">
                            <template #default="scope">
                                <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
                            </template>
                        </el-table-column>
                        <el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
                            <template #default="scope">
                                <ModifyRecord :data="scope.row" />
                            </template>
                        </el-table-column>
                        <el-table-column label="Operation" width="210" fixed="right" align="center">
                            <template #default="scope">
                                <el-button icon="ele-Edit" text type="primary" @click="openEditOrg(scope.row)"
                                    v-auth="'sysOrg:update'"> Edit </el-button>
                                <el-button icon="ele-Delete" text type="danger" @click="delOrg(scope.row)"
                                    v-auth="'sysOrg:delete'"> Delete </el-button>
                                <el-button icon="ele-CopyDocument" text type="primary" @click="openCopyOrg(scope.row)"
                                    v-auth="'sysOrg:add'"> Copy </el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-card>
            </el-splitter-panel>
        </el-splitter>

        <EditOrg ref="editOrgRef" :title="state.editOrgTitle" :orgData="state.orgTreeData" @reload="handleQuery" />
    </div>
</template>

<script lang="ts" setup name="sysOrg">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
//import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getAPI } from '/@/utils/axios-utils';
import { SysOrgApi } from '/@/api-services/api';
import { SysOrg, UpdateOrgInput } from '/@/api-services/models';
import OrgTree from '/@/views/system/org/component/orgTree.vue';
import EditOrg from '/@/views/system/org/component/editOrg.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

const editOrgRef = ref<InstanceType<typeof EditOrg>>();
const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const state = reactive({
    loading: false,
    tenantList: [] as Array<any>,
    orgData: [] as Array<SysOrg>, // Organization list data
    orgTreeData: [] as Array<SysOrg>, // All data of the organization tree
    queryParams: {
        id: 0,
        name: undefined,
        code: undefined,
        type: undefined,
    },
    tenantId: undefined,
    editOrgTitle: ''
});

onMounted(async () => {
    handleQuery();
});

// Query operation
const handleQuery = async (updateTree: boolean = false) => {
    state.loading = true;
    let res = await getAPI(SysOrgApi).apiSysOrgListGet(state.queryParams.id, state.queryParams.name, state.queryParams.code, state.queryParams.type);
    state.orgData = res.data.result ?? [];
    state.loading = false;
    // Whether to update the institution list tree on the left
    if (updateTree) {
        orgTreeRef.value?.initTreeData();
        // Update the organization list tree on the edit page
        res = await getAPI(SysOrgApi).apiSysOrgListGet(0);
        state.orgTreeData = res.data.result ?? [];
    }

    // If no node is selected and the query condition is empty, update the organization list tree on the edit page.
    if (state.queryParams.id == 0 && state.queryParams.name == undefined && state.queryParams.code == undefined && state.queryParams.type == undefined && !updateTree)
        state.orgTreeData = state.orgData;
};

// reset operation
const resetQuery = () => {
    state.queryParams.id = 0;
    state.queryParams.name = undefined;
    state.queryParams.code = undefined;
    state.queryParams.type = undefined;
    handleQuery();
};

// Open new page
const openAddOrg = () => {
    state.editOrgTitle = 'Add institution';
    editOrgRef.value?.openDialog({ status: 1, orderNo: 100, tenantId: state.tenantId });
};

// Open the edit page
const openEditOrg = (row: any) => {
    state.editOrgTitle = 'Editorial organization';
    editOrgRef.value?.openDialog(row);
};

// Open copy page
const openCopyOrg = (row: any) => {
    state.editOrgTitle = 'Copy organization';
    var copyRow = JSON.parse(JSON.stringify(row)) as UpdateOrgInput;
    copyRow.id = 0;
    copyRow.name = '';
    editOrgRef.value?.openDialog(copyRow);
};

// delete
const delOrg = (row: any) => {
    ElMessageBox.confirm(`Are you sure to delete the organization: [${row.name}]?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysOrgApi).apiSysOrgDeletePost({ id: row.id });
            ElMessage.success('Deleted successfully');
            handleQuery(true);
        })
        .catch(() => { });
};

// Tree component click
const nodeClick = async (node: any) => {
    state.queryParams.id = node.id;
    state.queryParams.name = undefined;
    state.queryParams.code = undefined;
    state.queryParams.type = undefined;
    state.tenantId = node.tenantId;
    handleQuery();
};
</script>
