<template>
    <div class="sys-notice-container">
        <el-card shadow="hover" :body-style="{ padding: 5 }">
            <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                <el-form-item label="title">
                    <el-input v-model="state.queryParams.title" placeholder="title" clearable />
                </el-form-item>
                <el-form-item label="Type">
                    <g-sys-dict v-model="state.queryParams.type" code="NoticeTypeEnum" render-as="select" clearable />
                </el-form-item>
                <el-form-item>
                    <el-button-group>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysNotice:page'"> Query </el-button>
                        <el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
                    </el-button-group>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Plus" @click="openAddNotice" v-auth="'sysNotice:add'"> Add New </el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 5px">
            <el-table :data="state.noticeData" v-loading="state.loading" border>
                <el-table-column type="index" label="No" width="55" align="center" />
                <el-table-column prop="title" label="title" width="250" header-align="center" show-overflow-tooltip />
                <el-table-column prop="content" label="content" header-align="center" show-overflow-tooltip>
                    <template #default="scope"> {{ removeHtml(scope.row.content) }} </template>
                </el-table-column>
                <el-table-column prop="type" label="Type" width="100" align="center">
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.type" code="NoticeTypeEnum" />
                    </template>
                </el-table-column>
                <el-table-column prop="createTime" label="Creation Time" width="180" align="center" />
                <el-table-column prop="status" label="state" width="100" align="center">
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.status" code="NoticeStatusEnum" />
                    </template>
                </el-table-column>
                <el-table-column prop="publicUserName" label="Publisher" width="130" align="center" />
                <el-table-column prop="publicTime" label="Release Time" width="180" align="center" />
                <el-table-column label="Operation" width="200" fixed="right" align="center">
                    <template #default="scope">
                        <el-button icon="ele-Position" size="small" text type="primary" @click="publicNotice(scope.row)" v-auth="'sysNotice:public'" :disabled="scope.row.status === 1"> release </el-button>
                        <el-button icon="ele-Edit" size="small" text type="primary" @click="openEditNotice(scope.row)" v-auth="'sysNotice:update'" :disabled="scope.row.status === 1"> Edit </el-button>
                        <el-button icon="ele-Delete" size="small" text type="danger" @click="delNotice(scope.row)" v-auth="'sysNotice:delete'" :disabled="scope.row.status === 1"> Delete </el-button>
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

        <EditNotice ref="editNoticeRef" :title="state.editNoticeTitle" @handleQuery="handleQuery" />
    </div>
</template>

<script lang="ts" setup name="sysNotice">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { SysNotice } from '/@/api-services/models';
import commonFunction from '/@/utils/commonFunction';
import EditNotice from '/@/views/system/notice/component/editNotice.vue';

const editNoticeRef = ref<InstanceType<typeof EditNotice>>();
const { removeHtml } = commonFunction();
const state = reactive({
    loading: false,
    noticeData: [] as Array<SysNotice>,
    queryParams: {
        title: undefined,
        type: undefined,
    },
    tableParams: {
        page: 1,
        pageSize: 50,
        total: 0 as any,
    },
    editNoticeTitle: '',
});

onMounted(async () => {
    handleQuery();
});

// Query operation
const handleQuery = async () => {
    state.loading = true;
    let params = Object.assign(state.queryParams, state.tableParams);
    var res = await getAPI(SysNoticeApi).apiSysNoticePagePost(params);
    state.noticeData = res.data.result?.items ?? [];
    state.tableParams.total = res.data.result?.total;
    state.loading = false;
};

// reset operation
const resetQuery = () => {
    state.queryParams.title = undefined;
    state.queryParams.type = undefined;
    handleQuery();
};

// Open new page
const openAddNotice = () => {
    state.editNoticeTitle = 'Add Notice';
    editNoticeRef.value?.openDialog({ type: 1 });
};

// Open the edit page
const openEditNotice = (row: any) => {
    state.editNoticeTitle = 'Edit Notice Announcement';
    editNoticeRef.value?.openDialog(row);
};

// delete
const delNotice = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to delete the notification: 【${row.title}】?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysNoticeApi).apiSysNoticeDeletePost({ id: row.id });
            handleQuery();
            ElMessage.success('Deleted successfully');
        })
        .catch(() => { });
};

// release
const publicNotice = (row: any) => {
    ElMessageBox.confirm(`Confirm to publish the notification announcement: 【${row.title}】, irreversible?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysNoticeApi).apiSysNoticePublicPost({ id: row.id });
            handleQuery();
            ElMessage.success('Published successfully');
        })
        .catch(() => { });
};

// Change page capacity
const handleSizeChange = (val: number) => {
    state.tableParams.pageSize = val;
    handleQuery();
};

// Change page number
const handleCurrentChange = (val: number) => {
    state.tableParams.page = val;
    handleQuery();
};
</script>
