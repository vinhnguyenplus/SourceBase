<template>
	<div class="sys-import-data-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="400px" @close="resetDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-UploadFilled /> </el-icon>
					<span> Data Import </span>
				</div>
			</template>
			
			<el-row :gutter="15" v-loading="state.loading">
				<el-col :span="24" class="mb10">
					<el-button icon="ele-Download" v-reclick="3000" @click="download" :disabled="state.loading">Template</el-button>
				</el-col>
				
				<el-col :span="24" class="mb15 flex">
					<el-upload
						:limit="1"
						:show-file-list="false"
						:auto-upload="false"
						:on-exceed="handleExceed"
						:on-change="handleFileChange"
						ref="uploadRef"
					>
						<template #trigger>
							<el-button class="mr10" type="primary" icon="ele-MostlyCloudy" :disabled="state.isCompleted || state.loading">Select File</el-button>
						</template>
					</el-upload>
					<span class="selected-file">{{ state.selectedFile ? state.selectedFile.name : 'No file selected' }}</span>
				</el-col>
				
				<!-- Error prompt area -->
				<el-col :span="24" v-if="state.importResultUrl" class="mt10">
					<div v-if="state.hasError" style="color: red; margin-bottom: 10px;">
						Import completed, there are some errors, please download the import results to view details
					</div>
					<el-link type="primary" :underline="false" @click="downloadImportResult">
						<el-icon class="mr5"><ele-Download /></el-icon>Download import results
					</el-link>
				</el-col>
			</el-row>

			<template #footer>
				<span class="dialog-footer">
					<el-button @click="closeDialog" :disabled="state.loading">Cancel</el-button>
					<el-button 
						type="primary" 
						@click="state.isCompleted ? closeDialog() : submitImport()"
						:disabled="(!state.selectedFile && !state.isCompleted) || state.loading"
					>
						{{ state.isCompleted ? 'Close' : 'Confirm' }}
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysImportData">
import type { UploadInstance, UploadProps, UploadRawFile, UploadFile } from 'element-plus'
import { ElUpload, ElMessage, genFileId } from 'element-plus';
import { downloadStreamFile } from '/@/utils/download';
import { reactive, ref } from 'vue';

const uploadRef = ref<UploadInstance>();
const state = reactive({
  isShowDialog: false,
  loading: false,
  isCompleted: false,
  hasError: false,
  selectedFile: null as File | null,
  importResultUrl: '' as string,
  importResultName: '' as string
});

// Define child components to pass values/events to parent components
const props = defineProps(['import', 'download']);
const emit = defineEmits(['refresh']);

// Open pop-up window
const openDialog = () => {
  resetDialog();
  state.isShowDialog = true;
};

// Reset dialog state
const resetDialog = () => {
  state.isCompleted = false;
  state.hasError = false;
  state.selectedFile = null;
  state.importResultUrl = '';
  state.importResultName = '';
  uploadRef.value?.clearFiles();
};

// Close pop-up window
const closeDialog = () => {
  state.isShowDialog = false;
  if (state.importResultUrl) {
    URL.revokeObjectURL(state.importResultUrl);
  }
};

// Select file exceeds limit event
const handleExceed: UploadProps['onExceed'] = (files) => {
  uploadRef.value!.clearFiles();
  const file = files[0] as UploadRawFile;
  file.uid = genFileId();
  uploadRef.value!.handleStart(file);
}

// File selection change event
const handleFileChange = (file: UploadFile) => {
  state.selectedFile = file.raw as File;
};

// Submit import
const submitImport = async () => {
  if (!state.selectedFile) return;
  
  try {
    state.loading = true;
    const res = await props.import(state.selectedFile);
    
    // Processing the import results
    const contentType = res.headers['content-type'] || '';
    if (contentType.includes('application/json')) {
      // JSON response handling (no error file)
      const decoder = new TextDecoder('utf-8');
      const data = decoder.decode(res.data);
      const result = JSON.parse(data);
      
      if (result.code === 200) {
        ElMessage.success(result.message);
        emit('refresh');
        state.hasError = false;
        closeDialog();  // Key modification: close the dialog box directly after successful import
      } else {
        ElMessage.error(result.message);
        state.hasError = false;
      }
    } else {
      // Binary response handling (with error files)
      const blob = new Blob([res.data]);
      const contentDisposition = res.headers['content-disposition'];
      let filename = 'Import Results.xlsx';
      
      if (contentDisposition) {
        const match = contentDisposition.match(/filename="?([^"]+)"?/);
        if (match && match[1]) {
          filename = decodeURIComponent(match[1]);
        }
      }
      
      // Clear old URLs
      if (state.importResultUrl) {
        URL.revokeObjectURL(state.importResultUrl);
      }
      
      // Create import result URL
      state.importResultUrl = URL.createObjectURL(blob);
      state.importResultName = filename;
      state.isCompleted = true;
      state.hasError = true;
      
	  //Refresh list display
	  emit('refresh');

      ElMessage.warning('Import completed, there are some errors');
    }
  } catch (error) {
    console.error('Import error:', error);
    ElMessage.error('An error occurred during the import process');
    state.hasError = false;
  } finally {
    state.loading = false;
  }
};

// Download import results
const downloadImportResult = () => {
  if (!state.importResultUrl) return;
  
  const link = document.createElement('a');
  link.href = state.importResultUrl;
  link.download = state.importResultName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  
  // Close the dialog box (optional, decide whether to keep it according to your needs)
  // closeDialog();
};

// Download template
const download = () => {
  props.download()
    .then((res: any) => downloadStreamFile(res))
    .catch((err: any) => ElMessage.error('Download error: ' + err));
}

// Export object
defineExpose({ openDialog, closeDialog });
</script>

<style scoped>
.selected-file {
  margin-left: 10px;
  line-height: 32px;
  color: var(--el-text-color-regular);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 200px;
}

.flex {
  display: flex;
  align-items: center;
}
</style>