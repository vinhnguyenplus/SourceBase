<template>
	<div class="sys-user-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-tabs v-loading="state.loading" v-model="state.selectedTabName">
				<el-tab-pane label="Basic Information" class="tab-pane">
					<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Account name" prop="account" :rules="[{ required: true, message: 'Account name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.account" placeholder="Account name" :disabled="state.ruleForm.id > 0" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Nickname">
									<el-input v-model="state.ruleForm.nickName" placeholder="Nickname" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Mobile phone number" prop="phone" :rules="[{ required: true, message: 'Mobile phone number cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.phone" placeholder="Mobile phone number" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Real Name" prop="realName" :rules="[{ required: true, message: 'Real name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.realName" placeholder="Real Name" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Account Type" prop="accountType" :rules="[{ required: true, message: 'Account type cannot be empty', trigger: 'blur' }]">
									<g-sys-dict
										v-model="state.ruleForm.accountType"
										:on-item-filter="(data: any) => data.code != 'SuperAdmin' && (data.code == 'SysAdmin' ? AccountTypeEnum.NUMBER_999 == userInfos.accountType : true)"
										code="AccountTypeEnum"
										render-as="select"
									/>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Email">
									<el-input v-model="state.ruleForm.email" placeholder="Email" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Language" prop="langCode" :rules="[{ required: true, message: 'Language cannot be empty', trigger: 'blur' }]">
									<el-select clearable filterable v-model="state.ruleForm.langCode" placeholder="Please select a language">
										<el-option v-for="(item, index) in state.languages" :key="index" :value="item.code" :label="item.label" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Personalized Homepage" prop="homepage">
									<el-input v-model="state.ruleForm.homepage" placeholder="Personalized Homepage" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb5">
								<el-form-item label="Sort">
									<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">Institutional organization</div>
							</el-divider>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Affiliated institution" prop="orgId" :rules="[{ required: true, message: 'Affiliation cannot be empty', trigger: 'blur' }]">
									<el-cascader :options="state.orgTreeData" :props="cascaderProps" placeholder="Affiliated institution" clearable filterable class="w100" v-model="state.ruleForm.orgId">
										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Position" prop="posId" :rules="[{ required: true, message: 'Job title cannot be empty', trigger: 'blur' } ]">
									<el-select v-model="state.ruleForm.posId" placeholder="Position" class="w100">
										<el-option v-for="d in state.posData" :key="d.id" :label="d.name" :value="d.id" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Job number">
									<el-input v-model="state.ruleForm.jobNum" placeholder="Job number" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Domain account">
									<el-input v-model="state.ruleForm.domainAccount" placeholder="Domain account" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Date of Joining">
									<el-date-picker v-model="state.ruleForm.joinDate" type="date" placeholder="Date of Joining" format="YYYY-MM-DD" value-format="YYYY-MM-DD" class="w100" />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">Affiliates</div>
							</el-divider>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-button icon="ele-Plus" type="primary" text plain @click="addExtOrgRow"> Increase affiliated institutions </el-button>
								<span style="font-size: 12px; color: gray; padding-left: 5px"> Data access permissions for the corresponding organizational structure </span>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="unique-box">
								<template v-if="state.ruleForm.extOrgIdList != undefined && state.ruleForm.extOrgIdList.length > 0">
                                    <div v-for="(v, k) in state.ruleForm.extOrgIdList" :key="k" class="unique-line">
                                        <el-form-item label="mechanism" label-width="55" :prop="`extOrgIdList[${k}].orgId`" :rules="[{ required: true, message: `Organization cannot be empty`, trigger: 'blur' }]">
											<el-cascader :options="props.orgTreeData" :props="cascaderProps" placeholder="Institutional organization" clearable filterable class="w100" v-model="state.ruleForm.extOrgIdList[k].orgId">
												<template #default="{ node, data }">
													<span>{{ data.name }}</span>
													<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
												</template>
											</el-cascader>
										</el-form-item>
                                        <el-form-item label="Position" label-width="55" :prop="`extOrgIdList[${k}].posId`" :rules="[{ required: true, message: `Position cannot be empty`, trigger: 'blur' }]">
											<el-select v-model="state.ruleForm.extOrgIdList[k].posId" placeholder="Job title" class="w100">
												<el-option v-for="d in state.posData" :key="d.id" :label="d.name" :value="d.id" />
											</el-select>
										</el-form-item>
                                        <div class="delete-btn">
                                            <el-button icon="ele-Delete" type="danger" circle plain size="small" @click="deleteExtOrgRow(k)" />
                                        </div>
                                    </div>
								</template>
								<el-empty :image-size="50" style="padding: 0px;" v-else></el-empty>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="Role Authorization" class="tab-pane">
					<el-transfer :data="state.roleData" :props="{ key: 'id', label: 'name' }" v-model="state.ruleForm.roleIdList" :titles="['Unauthorized', 'Authorized']"></el-transfer>
				</el-tab-pane>
				<el-tab-pane label="Archive information" class="tab-pane">
					<el-form :model="state.ruleForm" label-width="auto">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Type of ID" prop="cardType">
									<g-sys-dict v-model="state.ruleForm.cardType" code="CardTypeEnum" render-as="select" placeholder="Type of ID" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="ID number">
									<el-input v-model="state.ruleForm.idCardNum" placeholder="ID number" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="date of birth" prop="birthday">
									<el-date-picker v-model="state.ruleForm.birthday" type="date" placeholder="date of birth" format="YYYY-MM-DD" value-format="YYYY-MM-DD" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="gender">
									<g-sys-dict v-model="state.ruleForm.sex" code="GenderEnum" render-as="radio" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb5">
								<el-form-item label="age">
									<el-input-number v-model="state.ruleForm.age" placeholder="age" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="ethnic group">
									<g-sys-dict v-model="state.ruleForm.nation" code="NationEnum" render-as="select" placeholder="ethnic group" class="w100" clearable/>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="address">
									<el-input v-model="state.ruleForm.address" placeholder="address" clearable type="textarea" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Graduation school">
									<el-input v-model="state.ruleForm.college" placeholder="Graduation school" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Education level">
									<g-sys-dict v-model="state.ruleForm.cultureLevel" code="CultureLevelEnum" render-as="select" placeholder="Education level" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="political outlook">
									<el-input v-model="state.ruleForm.politicalOutlook" placeholder="political outlook" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Office phone">
									<el-input v-model="state.ruleForm.officePhone" placeholder="Office phone" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Emergency Contact">
									<el-input v-model="state.ruleForm.emergencyContact" placeholder="Emergency Contact" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Contact phone number">
									<el-input v-model="state.ruleForm.emergencyPhone" placeholder="Contact phone number" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Contact address">
									<el-input v-model="state.ruleForm.emergencyAddress" placeholder="Contact address" clearable type="textarea" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Remarks">
									<el-input v-model="state.ruleForm.remark" placeholder="Remarks" clearable type="textarea" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditUser">
import { onMounted, reactive, ref } from 'vue';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi, SysRoleApi, SysUserApi } from '/@/api-services/api';
import {AccountTypeEnum, RoleOutput, OrgTreeOutput, SysPos, UpdateUserInput} from '/@/api-services/models';
import { useLangStore } from '/@/stores/useLangStore';
const langStore = useLangStore();

const props = defineProps({
	title: String,
	orgTreeData: Array<OrgTreeOutput>,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const storesUserInfo = useUserInfo();
const { userInfos } = storeToRefs(storesUserInfo);
const state = reactive({
	loading: false,
	isShowDialog: false,
	selectedTabName: '0', // Selected tab page
	orgTreeData: [] as Array<OrgTreeOutput>,
	ruleForm: {} as UpdateUserInput,
	posData: [] as Array<SysPos>, // Job data
	roleData: [] as Array<RoleOutput>, // character data
	languages: [] as any[], // Linguistic data
});
// Cascading selector configuration options
const cascaderProps = { checkStrictly: true, emitPath: false, value: 'id', label: 'name', expandTrigger: 'hover' };

onMounted(async () => {
	state.loading = true;
	var res = await getAPI(SysPosApi).apiSysPosListGet();
	state.posData = res.data.result ?? [];
	var res1 = await getAPI(SysRoleApi).apiSysRoleListGet();
	state.roleData = res1.data.result ?? [];
	if (langStore.languages.length === 0) {
        await langStore.loadLanguages();
    }
	state.languages = langStore.languages;
	state.loading = false;
});

// Open pop-up window
const openDialog = async (row: any) => {
	state.orgTreeData = (row.tenantId ? props.orgTreeData?.filter((e) => e.tenantId === row.tenantId) : props.orgTreeData) ?? [];
	state.posData = (row.tenantId ? state.posData?.filter((e) => e.tenantId === row.tenantId) : state.posData) ?? [];
	state.roleData = (row.tenantId ? state.roleData?.filter((e) => e.tenantId === row.tenantId) : state.roleData) ?? [];
	ruleFormRef.value?.resetFields();
	state.selectedTabName = '0'; // Reset to first tab page
	state.ruleForm = JSON.parse(JSON.stringify(row));
	if (row.id != undefined) {
		var resRole = await getAPI(SysUserApi).apiSysUserOwnRoleListUserIdGet(row.id);
		state.ruleForm.roleIdList = resRole.data.result;
		var resExtOrg = await getAPI(SysUserApi).apiSysUserOwnExtOrgListUserIdGet(row.id);
		state.ruleForm.extOrgIdList = resExtOrg.data.result;
	} else state.ruleForm.accountType = 777; // Default common account type
	state.isShowDialog = true;
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
			await getAPI(SysUserApi).apiSysUserUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysUserApi).apiSysUserAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Add affiliate row
const addExtOrgRow = () => {
	if (state.ruleForm.extOrgIdList == undefined) state.ruleForm.extOrgIdList = [];
	state.ruleForm.extOrgIdList?.push({});
};

// Delete affiliate row
const deleteExtOrgRow = (k: number) => {
	state.ruleForm.extOrgIdList?.splice(k, 1);
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.tab-pane {
    padding: 0 10px;
    height: 570px;
    overflow: hidden auto;

    .el-transfer {
        margin: 0 auto;
        width: fit-content;
        height: 100%;

        :deep(.el-transfer-panel) {
            height: 100%;
        }

        --el-transfer-panel-body-height: calc(100% - 40px);
    }

    .unique-box {
        display: grid;
        gap: 20px;
    }
    .unique-line {
        display: flex;
        gap: 20px;
        
        .el-form-item {
            margin-bottom: 0;
            flex: 1;
        }

        .delete-btn {
            align-self: center;
            width: 24px;
            margin-left: -10px;
        }
    }
}
</style>