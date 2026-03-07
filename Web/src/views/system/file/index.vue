<template>
	<div class="sys-file-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="File name" prop="fileName">
					<el-input v-model="state.queryParams.fileName" placeholder="File name" clearable />
				</el-form-item>
				<el-form-item label="start time" prop="name">
					<el-date-picker v-model="state.queryParams.startTime" type="datetime" placeholder="start time" value-format="YYYY-MM-DD HH:mm:ss" />
				</el-form-item>
				<el-form-item label="end time" prop="code">
					<el-date-picker v-model="state.queryParams.endTime" type="datetime" placeholder="end time" value-format="YYYY-MM-DD HH:mm:ss" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysFile:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openUploadDialog" v-auth="'sysFile:uploadFile'"> upload </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.fileData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="fileName" label="name" min-width="150" header-align="center" show-overflow-tooltip />
				<el-table-column prop="suffix" label="Suffix" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag round>{{ scope.row.suffix }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="sizeKb" label="Size kb" align="center" show-overflow-tooltip />
				<el-table-column prop="url" label="Preview" align="center">
					<template #default="scope">
						<el-image
							style="width: 60px; height: 60px"
							:src="getFileUrl(scope.row)"
							alt="Unable to preview"
							:lazy="true"
							:hide-on-click-modal="true"
							:preview-src-list="[getFileUrl(scope.row)]"
							:initial-index="0"
							fit="scale-down"
							preview-teleported
						>
							<template #error> </template>
						</el-image>
					</template>
				</el-table-column>
				<el-table-column prop="bucketName" label="storage location" align="center" show-overflow-tooltip />
				<el-table-column prop="id" label="Storage ID" align="center" show-overflow-tooltip />
				<el-table-column prop="fileType" label="File type" min-width="100" header-align="center" show-overflow-tooltip />
				<el-table-column prop="isPublic" label="Is it public?" min-width="100" header-align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.isPublic === true" type="success">Yes</el-tag>
						<el-tag v-else type="danger">no</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="relationName" label="Associated Object Name" min-width="150" align="center" />
				<el-table-column prop="relationId" label="Associated object ID" align="center" />
				<el-table-column prop="belongId" label="BelongingId" align="center" />
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="260" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button-group>
							<el-button icon="ele-View" size="small" type="primary" @click="openFilePreviewDialog(scope.row)" v-auth="'sysFile:delete'"></el-button>
							<el-button icon="ele-Download" size="small" type="primary" @click="downloadFile(scope.row)" v-auth="'sysFile:downloadFile'"></el-button>
							<el-button icon="ele-Delete" size="small" type="danger" @click="delFile(scope.row)" v-auth="'sysFile:delete'"></el-button>
							<el-button icon="ele-Edit" size="small" type="primary" @click="openEditSysFile(scope.row)" v-auth="'sysFile:update'"></el-button>
						</el-button-group>
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

		<el-dialog v-model="state.dialogUploadVisible" :lock-scroll="false" draggable width="400px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-UploadFilled /> </el-icon>
					<span> Upload File </span>
				</div>
			</template>
			<div>
				<el-select v-model="state.fileType" placeholder="Please select the file type" style="margin-bottom: 10px">
					<el-option label="Related documents" value="Related documents" />
					<el-option label="Archived File" value="Archived File" />
				</el-select>
				Public or not:
				<el-radio-group v-model="state.isPublic">
					<el-radio :value="false">no</el-radio>
					<el-radio :value="true">Yes</el-radio>
				</el-radio-group>
				<el-upload ref="uploadRef" drag :auto-upload="false" :limit="1" :file-list="state.fileList" action :on-change="handleChange" accept=".jpg,.png,.bmp,.gif,.txt,.xml,.pdf,.xlsx,.docx">
					<el-icon class="el-icon--upload">
						<ele-UploadFilled />
					</el-icon>
					<div class="el-upload__text">Drag the file here, or<em>Click to upload</em></div>
					<template #tip>
						<div class="el-upload__tip">Please upload a file no larger than 10MB in size</div>
					</template>
				</el-upload>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.dialogUploadVisible = false">Cancel</el-button>
					<el-button type="primary" @click="uploadFile">Confirm</el-button>
				</span>
			</template>
		</el-dialog>

		<el-drawer :title="state.fileName" v-model="state.dialogDocxVisible" size="50%" destroy-on-close>
			<vue-office-docx :src="state.docxUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-drawer :title="state.fileName" v-model="state.dialogXlsxVisible" size="50%" destroy-on-close>
			<vue-office-excel :src="state.excelUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-drawer :title="state.fileName" v-model="state.dialogPdfVisible" size="50%" destroy-on-close>
			<vue-office-pdf :src="state.pdfUrl" style="height: 100vh" @rendered="renderedHandler" @error="errorHandler" />
		</el-drawer>
		<el-image-viewer v-if="state.showViewer" :url-list="state.previewList" :hideOnClickModal="true" @close="state.showViewer = false"></el-image-viewer>
		<EditSysFile ref="editSysFileRef" title="Edit file" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysFile">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, UploadInstance } from 'element-plus';
import VueOfficeDocx from '@vue-office/docx';
import VueOfficeExcel from '@vue-office/excel';
import VueOfficePdf from '@vue-office/pdf';
import '@vue-office/docx/lib/index.css';
import '@vue-office/excel/lib/index.css';

import EditSysFile from '/@/views/system/file/component/editSysfile.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { downloadByUrl } from '/@/utils/download';
import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/api';
import { SysFile } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

// const baseUrl = window.__env__.VITE_API_URL;
const userStore = useUserInfo();
const uploadRef = ref<UploadInstance>();
const editSysFileRef = ref<InstanceType<typeof EditSysFile>>();
const state = reactive({
	loading: false,
	fileData: [] as Array<SysFile>,
	queryParams: {
		tenantId: undefined,
		fileName: undefined,
		startTime: undefined,
		endTime: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	dialogUploadVisible: false,
	diaglogEditFile: false,
	fileList: [] as any,
	dialogDocxVisible: false,
	dialogXlsxVisible: false,
	dialogPdfVisible: false,
	showViewer: false,
	docxUrl: '',
	excelUrl: '',
	pdfUrl: '',
	fileName: '',
	fileType: '',
	isPublic: false,
	previewList: [] as string[],
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
	if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;

	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysFileApi).apiSysFilePagePost(params);
	console.log(res);
	state.fileData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.fileName = undefined;
	state.queryParams.startTime = undefined;
	state.queryParams.endTime = undefined;
	handleQuery();
};

// Open upload page
const openUploadDialog = () => {
	state.fileList = [];
	state.dialogUploadVisible = true;
	state.isPublic = false;
};

// Get the file list through onChanne method
const handleChange = (file: any, fileList: []) => {
	state.fileList = fileList;
};

// upload
const uploadFile = async () => {
	if (state.fileList.length < 1) return;
	await getAPI(SysFileApi).apiSysFileUploadFilePostForm(state.fileList[0].raw, state.fileType, state.isPublic, undefined);
	handleQuery();
	ElMessage.success('Upload successful');
	state.dialogUploadVisible = false;
};

// download
const downloadFile = async (row: any) => {
	// var res = await getAPI(SysFileApi).sysFileDownloadPost({ id: row.id });
	var fileUrl = getFileUrl(row);
	downloadByUrl({ url: fileUrl });
};

// delete
const delFile = (row: any) => {
	ElMessageBox.confirm(`Are you sure you want to delete the file: 【${row.fileName}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysFileApi).apiSysFileDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Open the file preview page
const openFilePreviewDialog = async (row: any) => {
	if (row.suffix == '.pdf') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.pdfUrl = getFileUrl(row);
		state.dialogPdfVisible = true;
	} else if (row.suffix == '.docx') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.docxUrl = getFileUrl(row);
		state.dialogDocxVisible = true;
	} else if (row.suffix == '.xlsx') {
		state.fileName = `【${row.fileName}${row.suffix}】`;
		state.excelUrl = getFileUrl(row);
		state.dialogXlsxVisible = true;
	} else if (['.jpg', '.png', '.jpeg', '.bmp'].findIndex((e) => e == row.suffix) > -1) {
		state.previewList = [getFileUrl(row)];
		state.showViewer = true;
	} else {
		ElMessage.error('This file format does not support preview');
	}
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

// Get file address
const getFileUrl = (row: SysFile): string => {
	if (row.bucketName == 'Local') {
		return `/${row.filePath}/${row.id}${row.suffix}`;
	} else {
		return row.url!;
	}
};

// Open the edit page
const openEditSysFile = (row: any) => {
	editSysFileRef.value?.openDialog(row);
};

// File rendering completed
const renderedHandler = () => {};
// File rendering failed
const errorHandler = () => {};
</script>
