<template>
	<div class="sys-menu-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Previous menu">
							<el-cascader :options="props.menuData" :props="cascaderProps" placeholder="Please select the upper level menu" clearable filterable class="w100" v-model="state.ruleForm.pid">
								<template #default="{ node, data }">
									<span>{{ data.title }}</span>
									<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
								</template>
							</el-cascader>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="menuType" prop="type" :rules="[{ required: true, message: 'Menu type cannot be empty', trigger: 'blur' }]">
							<g-sys-dict v-model="state.ruleForm.type" code="MenuTypeEnum" render-as="radio" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Menu name" prop="title" :rules="[{ required: true, message: 'Menu name cannot be empty', trigger: 'blur' } ]">
							<g-multi-lang-Input entityName="SysMenu" fieldName="Title" :entityId="Number(state.ruleForm.id)" v-model="state.ruleForm.title" placeholder="Menu name" clearable />
						</el-form-item>
					</el-col>
					<template v-if="state.ruleForm.type === 1 || state.ruleForm.type === 2">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Route Name">
								<el-input v-model="state.ruleForm.name" placeholder="Route Name" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="routing path">
								<el-input v-model="state.ruleForm.path" placeholder="routing path" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="component path">
								<el-input v-model="state.ruleForm.component" placeholder="component path" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Menu icon">
								<IconSelector v-model="state.ruleForm.icon" :size="getGlobalComponentSize" placeholder="Menu icon" type="all" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Redirect">
								<el-input v-model="state.ruleForm.redirect" placeholder="Redirectaddress" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Link address">
								<el-input v-model="state.ruleForm.outLink" placeholder="External link/embedded link address" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Menu sorting">
								<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Whether to hide">
								<el-radio-group v-model="state.ruleForm.isHide">
									<el-radio :value="true">hide</el-radio>
									<el-radio :value="false">Do not hide</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Whether to cache">
								<el-radio-group v-model="state.ruleForm.isKeepAlive">
									<el-radio :value="true">cache</el-radio>
									<el-radio :value="false">Nocache</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Is it fixed?">
								<el-radio-group v-model="state.ruleForm.isAffix">
									<el-radio :value="true">fixed</el-radio>
									<el-radio :value="false">Not fixed</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="YesnoEmbedded">
								<el-radio-group v-model="state.ruleForm.isIframe">
									<el-radio :value="true">Embedded</el-radio>
									<el-radio :value="false">Not embedded</el-radio>
								</el-radio-group>
							</el-form-item>
						</el-col>
					</template>
					<template v-if="state.ruleForm.type === 3">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Permission Identifier">
								<el-input v-model="state.ruleForm.permission" placeholder="Permission Identifier" clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="Menu sorting">
								<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
							</el-form-item>
						</el-col>
					</template>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Enable or not">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :value="1">enable</el-radio>
								<el-radio :value="2">Not enabled</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditMenu">
import { computed, reactive, ref } from 'vue';
import IconSelector from '/@/components/iconSelector/index.vue';
import other from '/@/utils/other';
import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi } from '/@/api-services/api';
import { SysMenu, UpdateMenuInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
	menuData: Array<SysMenu>,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateMenuInput,
});
// Cascading selector configuration options
const cascaderProps = { checkOnClickNode: true, checkStrictly: true, emitPath: false, value: 'id', label: 'title' };

// Get global component size
const getGlobalComponentSize = computed(() => {
	return other.globalComponentSize();
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery');
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
			await getAPI(SysMenuApi).apiSysMenuUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysMenuApi).apiSysMenuAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
