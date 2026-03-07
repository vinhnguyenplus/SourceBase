<template>
    <div class="notice-container">
        <el-card shadow="hover" :body-style="{ padding: 5 }">
            <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                <el-form-item label="title">
                    <el-input v-model="state.queryParams.title" placeholder="title" clearable />
                </el-form-item>
                <el-form-item label="Type">
                    <el-select v-model="state.queryParams.type" placeholder="Type" clearable>
                        <el-option label="Notice" :value="1" />
                        <el-option label="Announcement" :value="2" />
                    </el-select>
                </el-form-item>
                <el-form-item>
                    <el-button-group>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery"> Query </el-button>
                        <el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
                    </el-button-group>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 5px">
            <el-table :data="state.noticeData" style="width: 100%" v-loading="state.loading" border
                :row-class-name="tableRowClassName">
                <el-table-column type="index" label="No" width="55" align="center" />
                <el-table-column prop="sysNotice.title" label="title" width="250" header-align="center" show-overflow-tooltip />
                <el-table-column prop="sysNotice.content" label="content" header-align="center" show-overflow-tooltip>
                    <template #default="scope"> {{ removeHtml(scope.row.sysNotice.content) }} </template>
                </el-table-column>
                <el-table-column prop="sysNotice.type" label="Type" width="100" align="center">
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.sysNotice.type" code="NoticeTypeEnum" />
                    </template>
                </el-table-column>
                <el-table-column prop="sysNotice.createTime" label="Creation Time" width="180" align="center" />
                <el-table-column prop="readStatus" label="Reading status" width="100" align="center">
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.readStatus" code="NoticeUserStatusEnum" />
                    </template>
                </el-table-column>
                <el-table-column prop="sysNotice.publicUserName" label="Publisher" width="130" align="center" />
                <el-table-column prop="sysNotice.publicTime" label="Release Time" width="180" align="center" />
                <el-table-column label="Operation" width="100" align="center" fixed="right">
                    <template #default="scope">
                        <el-button icon="ele-InfoFilled" size="small" text type="primary" @click="viewDetail(scope.row)"> Details </el-button>
                    </template>
                </el-table-column>
            </el-table>
            <el-pagination size="small" background layout="total, sizes, prev, pager, next, jumper" 
                v-model:currentPage="state.tableParams.page" 
                v-model:page-size="state.tableParams.pageSize"
                :total="state.tableParams.total" 
                :page-sizes="[10, 20, 50, 100]"
                @size-change="handleSizeChange" 
                @current-change="handleCurrentChange"
            />
        </el-card>
        <el-dialog v-model="state.dialogVisible" draggable width="769px">
            <template #header>
                <div style="color: #fff">
                    <el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Bell /></el-icon>
                    <span> Message Details </span>
                </div>
            </template>
            <div class="w-e-text-container">
                <div v-html="state.content" data-slate-editor></div>
            </div>
            <template #footer>
                <span class="dialog-footer">
                    <el-button type="primary" @click="state.dialogVisible = false">Confirm</el-button>
                </span>
            </template>
        </el-dialog>
    </div>
</template>

<script setup lang="ts" name="notice">
import '@wangeditor/editor/dist/css/style.css';
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { SysNoticeUser } from '/@/api-services/models';
import commonFunction from '/@/utils/commonFunction';

const { removeHtml } = commonFunction();
const state = reactive({
    loading: false,
    noticeData: [] as Array<SysNoticeUser>,
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
    dialogVisible: false,
    content: '',
});
onMounted(async () => {
    handleQuery();
});

// Query operation
const handleQuery = async () => {
    state.loading = true;
    const pageNoticeInput = {
        title: state.queryParams.title,
        type: state.queryParams.type,
        page: state.tableParams.page,
        pageSize: state.tableParams.pageSize
    };
    var res = await getAPI(SysNoticeApi).apiSysNoticePageReceivedPost(pageNoticeInput);
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
// check the details
const viewDetail = async (row: any) => {
    state.content = row.sysNotice.content;
    state.dialogVisible = true;

    row.readStatus = 1;
    // mittBus.emit('noticeRead', row.sysNotice.id);
    await getAPI(SysNoticeApi).apiSysNoticeSetReadPost({ id: row.sysNotice.id });
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const tableRowClassName = ({ row, rowIndex }: { row: SysNoticeUser; rowIndex: number }) => {
    return row.readStatus === 1 ? 'info-row' : '';
};
</script>

<style lang="scss">
// .el-table .info-row {
// 	--el-table-tr-bg-color: var(--el-color-info-light-9);
// }
</style>
