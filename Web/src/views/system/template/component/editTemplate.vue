<template>
	<div class="sys-config-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef">
				<el-tabs v-model="state.selectedTabName">
					<el-tab-pane label="Basic Information" name="1" >
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="name" prop="name" :rules="[{ required: true, message: 'Name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.name" placeholder="name" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Encoding" prop="code" :rules="[{ required: true, message: 'Encoding cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.code" placeholder="Encoding" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Group" prop="groupName" :rules="[{ required: true, message: 'Group cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.groupName" placeholder="Group" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Type" prop="type" :rules="[{ required: true, message: 'Type cannot be empty', trigger: 'blur' }]">
									<g-sys-dict v-model="state.ruleForm.type" code="TemplateTypeEnum" render-as="select"/>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Sort">
									<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Remarks">
									<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="Template content" name="2">
						<el-row :gutter="5">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Content Type">
									<el-radio-group v-model="state.contentType">
										<el-radio :value="1">Rich Text</el-radio>
										<el-radio :value="2">puretext</el-radio>
									</el-radio-group>
									<el-button class="ml35" @click="() => editorRef?.ref?.insertNode({ text: '@(name)' })">Insert Parameter</el-button>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="content" prop="content" :rules="[{ required: true, message: 'Content cannot be empty', trigger: 'blur' }]" label-position="top">
									<Editor v-model:get-html="state.ruleForm.content" ref="editorRef" height="200px" v-if="state.contentType == 1" />
									<el-input v-model="state.ruleForm.content" v-else type="textarea" :rows="15" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" style="user-select: none;">
								<el-row :gutter="5">
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" title="Double-click to delete parameter item">
										<el-form-item label="Preview parameters" label-position="top">
											<el-button icon="ele-Plus" text @click="() => state.renderData.push([])"></el-button>
										</el-form-item>
									</el-col>
									<el-col v-for="(item, index) in state.renderData" :key="index" :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb5" @dblclick="() => state.renderData.splice(index, 1)">
										<el-row :gutter="5">
											<el-col :span="8">
												<el-input v-model="item[0]" :placeholder="'ParameterName' + (index + 1)"/>
											</el-col>
											<el-col :span="16">
												<el-input v-model="item[1]" :placeholder="'Parameter Value' + (index + 1)"/>
											</el-col>
										</el-row>
									</el-col>
								</el-row>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Preview results:" label-width="85" />
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<span v-html="state.result"></span>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button @click="showPreView" v-reclick="3000">Preview</el-button>
					<el-button type="primary" @click="submit" v-reclick="2000">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditConfig">
import {reactive, ref, watch} from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysTemplateApi } from '/@/api-services/api';
import { UpdateTemplateInput } from '/@/api-services/models';
import Editor from "/@/components/editor/index.vue";
import GSysDict from "/@/components/sysDict/sysDict.vue";

const props = defineProps({
	title: String,
});
const editorRef = ref();
const ruleFormRef = ref();
const emits = defineEmits(['updateData']);
const state = reactive({
	isShowDialog: false,
	selectedTabName: "1",
	renderData: [] as any,
	result: '' as any,
	contentType: 1,
	ruleForm: {} as UpdateTemplateInput,
});

const getRenderData = () => {
	const data = {} as any;
	state.renderData.forEach((e: [string, string]) => data[e[0]] = e[1]);
	return data;
}

const getRenderContent = async () => {
	const res = await getAPI(SysTemplateApi).apiSysTemplateRenderPost({ content: state.ruleForm.content, data: getRenderData() })
	state.result = res.data.result;
}

const showPreView = () => {
	getRenderContent();
	state.selectedTabName = '2';
}

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.selectedTabName = "1";
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('updateData');
	state.isShowDialog = false;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysTemplateApi).apiSysTemplateUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysTemplateApi).apiSysTemplateAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

watch(
		() => state.ruleForm.content,
		() => {
			state.ruleForm.content?.match(/@\((.*?)\)/g)?.forEach((pa: string, index) => {
				const key = pa.substring(2, pa.length - 1);
				if (!state.renderData.find((e: [string, string]) => e[0] === key)) {
					state.renderData.push([key, 'Parameter' + (index + 1)]);
				}
			});
		}
)

// Export object
defineExpose({ openDialog });
</script>
