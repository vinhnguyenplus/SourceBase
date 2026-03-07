<template>
	<div class="sys-userCenter-container">
		<el-row :gutter="5" style="width: 100%">
			<el-col :span="8" :xs="24">
				<el-card shadow="hover">
					<div class="account-center-avatarHolder">
						<!-- <el-upload class="h100" ref="uploadAvatarRef" action :limit="1" :show-file-list="false" :auto-upload="false" :on-change="uploadAvatarFile" accept=".jpg,.png,.bmp,.gif">
							<el-avatar :size="100" :src="userInfos.avatar" />
						</el-upload> -->
						<el-avatar
							:size="100"
							:src="userInfos.avatar"
							@click="openCropperDialog"
							v-loading="state.avatarLoading"
							element-loading-spinner="el-icon-Upload"
							element-loading-background="rgba(0, 0, 0, 0.2)"
							@mouseenter="mouseEnterAvatar"
							@mouseleave="mouseLeaveAvatar"
						/>
						<div class="username">{{ userInfos.realName }}</div>
					</div>
					<div class="account-center-org">
						<p>
							<el-icon><ele-School /></el-icon> <span>{{ userInfos.orgName ?? 'Super Administrator' }}</span>
						</p>
						<p>
							<el-icon><ele-Mug /></el-icon> <span>{{ userInfos.posName ?? 'Super Administrator' }}</span>
						</p>
						<p>
							<el-icon><ele-LocationInformation /></el-icon> <span>{{ userInfos.address ?? 'Home address' }}</span>
						</p>
					</div>
					<div class="image-signature">
						<el-image :src="userInfos.signature" fit="contain" alt="Electronic signature" loading="lazy" style="width: 100%; height: 100%"> </el-image>
					</div>
					<el-button icon="ele-Edit" type="primary" @click="openSignDialog" v-auth="'sysFile:uploadSignature'"> Electronic signature </el-button>
					<el-upload
						ref="uploadSignRef"
						action
						accept=".png"
						:limit="1"
						:show-file-list="false"
						:auto-upload="false"
						:on-change="uploadSignFile"
						:on-exceed="uploadSignFileExceed"
						style="display: inline-block; margin-left: 12px; position: absolute"
					>
						<el-button icon="ele-UploadFilled" v-auth="'sysFile:uploadSignature'">Upload handwritten signature</el-button>
					</el-upload>
				</el-card>
			</el-col>

			<el-col :span="16" :xs="24">
				<el-card shadow="hover">
					<el-tabs>
						<el-tab-pane label="Basic Information" v-loading="state.loading">
							<el-form :model="state.ruleFormBase" ref="ruleFormBaseRef" label-width="auto">
								<el-row :gutter="35">
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Real Name" prop="realName" :rules="[{ required: true, message: 'Real name cannot be empty', trigger: 'blur' }]">
											<el-input v-model="state.ruleFormBase.realName" placeholder="Real Name" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Nickname">
											<el-input v-model="state.ruleFormBase.nickName" placeholder="Nickname" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Mobile phone number" prop="phone" :rules="[{ required: true, message: 'Mobile phone number cannot be empty', trigger: 'blur' }]">
											<el-input v-model="state.ruleFormBase.phone" placeholder="Mobile phone number" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Email">
											<el-input v-model="state.ruleFormBase.email" placeholder="Email" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="date of birth" prop="birthday" :rules="[{ required: true, message: 'Date of birth cannot be empty', trigger: 'blur' }]">
											<el-date-picker v-model="state.ruleFormBase.birthday" type="date" placeholder="date of birth" format="YYYY-MM-DD" value-format="YYYY-MM-DD" class="w100" />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="gender">
											<el-radio-group v-model="state.ruleFormBase.sex">
												<el-radio :value="1">male</el-radio>
												<el-radio :value="2">Female</el-radio>
											</el-radio-group>
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Language" prop="langCode" :rules="[{ required: true, message: 'Language cannot be empty', trigger: 'blur' }]">
											<el-select clearable filterable v-model="state.ruleFormBase.langCode" placeholder="Please select a language">
												<el-option v-for="(item, index) in state.languages" :key="index" :value="item.code" :label="item.label" />
											</el-select>
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
										<el-form-item label="Personalized Homepage" prop="homepage">
											<el-input v-model="state.ruleFormBase.homepage" placeholder="Personalized Homepage" clearable />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
										<el-form-item label="address">
											<el-input v-model="state.ruleFormBase.address" placeholder="address" clearable type="textarea" />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
										<el-form-item label="Remarks">
											<el-input v-model="state.ruleFormBase.remark" placeholder="Remarks" clearable type="textarea" />
										</el-form-item>
									</el-col>
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
										<el-form-item>
											<el-button icon="ele-SuccessFilled" type="primary" @click="submitUserBase" v-auth="'sysUser:baseInfo'"> Save basic information </el-button>
										</el-form-item>
									</el-col>
								</el-row>
							</el-form>
						</el-tab-pane>
						<el-tab-pane label="Organizational structure">
							<OrgTree ref="orgTreeRef" />
						</el-tab-pane>
						<el-tab-pane label="Change Password">
							<el-form ref="ruleFormPasswordRef" :model="state.ruleFormPassword" label-width="auto">
								<el-form-item label="Current Password" prop="passwordOld" :rules="[{ required: true, message: 'The current password cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleFormPassword.passwordOld" type="password" autocomplete="off" show-password />
								</el-form-item>
								<el-form-item label="New password" prop="passwordNew" :rules="[{ required: true, message: 'New password cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleFormPassword.passwordNew" type="password" autocomplete="off" show-password />
								</el-form-item>
								<el-form-item label="Confirm password" prop="passwordNew2" :rules="[{ validator: validatePassword, required: true, trigger: 'blur' }]">
									<el-input v-model="state.passwordNew2" type="password" autocomplete="off" show-password />
								</el-form-item>
								<el-form-item>
									<el-button icon="ele-Refresh" @click="resetPassword">Reset</el-button>
									<el-button icon="ele-SuccessFilled" type="primary" @click="submitPassword" v-auth="'sysUser:changePwd'">Confirm</el-button>
								</el-form-item>
							</el-form>
						</el-tab-pane>
					</el-tabs>
				</el-card>
			</el-col>
		</el-row>

		<el-dialog v-model="state.signDialogVisible" draggable width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-EditPen /> </el-icon>
					<span> Electronic signature </span>
				</div>
			</template>
			<div style="border: 1px dashed gray; width: 100%; height: 250px">
				<VueSignaturePad ref="signaturePadRef" :options="state.signOptions" style="background-color: #fff" />
			</div>
			<div style="margin-top: 10px">
				<div style="display: inline">Brush Thickness:<el-input-number v-model="state.signOptions.minWidth" :min="0.5" :max="2.5" :step="0.1" size="small" /></div>
				<div style="display: inline; margin-left: 30px">Brush color:<el-color-picker v-model="state.signOptions.penColor" color-format="hex" size="default"> </el-color-picker></div>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="unDoSign">Revoke</el-button>
					<el-button @click="clearSign">Clear screen</el-button>
					<el-button type="primary" @click="saveUploadSign">save</el-button>
				</span>
			</template>
		</el-dialog>

		<CropperDialog ref="cropperDialogRef" :title="state.cropperTitle" @uploadCropperImg="uploadCropperImg" />
	</div>
</template>

<script lang="ts" setup name="sysUserCenter">
import { onMounted, watch, reactive, ref } from 'vue';
import { storeToRefs } from 'pinia';
import { ElForm, ElMessageBox, genFileId } from 'element-plus';
import type { UploadInstance, UploadProps, UploadRawFile } from 'element-plus';
import { useUserInfo } from '/@/stores/userInfo';
import { base64ToFile, blobToFile } from '/@/utils/base64Conver';
import OrgTree from '/@/views/system/user/component/orgTree.vue';
import CropperDialog from '/@/components/cropper/index.vue';
import VueGridLayout from 'vue-grid-layout';
import { sm2 } from 'sm-crypto-v2';
import { accessTokenKey, clearAccessAfterReload, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysFileApi, SysUserApi } from '/@/api-services/api';
import { ChangePwdInput, SysUser, SysFile } from '/@/api-services/models';
import { useLangStore } from '/@/stores/useLangStore';
import { Local } from '/@/utils/storage';
const langStore = useLangStore();
const stores = useUserInfo();
const { userInfos } = storeToRefs(stores);
const uploadSignRef = ref<UploadInstance>();
//const uploadAvatarRef = ref<UploadInstance>();
const signaturePadRef = ref<InstanceType<typeof VueGridLayout>>();
const ruleFormBaseRef = ref<InstanceType<typeof ElForm>>();
const ruleFormPasswordRef = ref<InstanceType<typeof ElForm>>();
const cropperDialogRef = ref<InstanceType<typeof CropperDialog>>();
const state = reactive({
	loading: false,
	avatarLoading: false,
	signDialogVisible: false,
	ruleFormBase: {} as SysUser,
	ruleFormPassword: {} as ChangePwdInput,
	signOptions: {
		penColor: '#000000',
		minWidth: 1.0,
		onBegin: () => {
			signaturePadRef.value.resizeCanvas();
		},
	},
	signFileList: [] as any,
	passwordNew2: '',
	cropperTitle: '',
	languages: [] as any[], // Linguistic data
});

onMounted(async () => {
	state.loading = true;
	var res = await getAPI(SysUserApi).apiSysUserBaseInfoGet();
	state.ruleFormBase = res.data.result ?? { account: '' };
	if (langStore.languages.length === 0) {
        await langStore.loadLanguages();
    }
	state.languages = langStore.languages;
	state.loading = false;
});

watch(state.signOptions, () => {
	signaturePadRef.value.signaturePad.penColor = state.signOptions.penColor;
	signaturePadRef.value.signaturePad.minWidth = state.signOptions.minWidth;
});

// Upload avatar image
const uploadCropperImg = async (e: any) => {
	var res = await getAPI(SysFileApi).apiSysFileUploadAvatarPostForm(blobToFile(e.img, userInfos.value.account + '.png'));
	userInfos.value.avatar = getFileUrl(res.data.result!);
	state.ruleFormBase.avatar = userInfos.value.avatar;
};

// Open the electronic signature page
const openSignDialog = () => {
	state.signDialogVisible = true;
};

// Save and upload electronic signature
const saveUploadSign = async () => {
	const { isEmpty, data } = signaturePadRef.value.saveSignature();
	if (isEmpty) {
		userInfos.value.signature = null;
		state.ruleFormBase.signature = null;
	} else {
		var res = await getAPI(SysFileApi).apiSysFileUploadSignaturePostForm(base64ToFile(data, userInfos.value.account + '.png'));
		userInfos.value.signature = getFileUrl(res.data.result!);
		state.ruleFormBase.signature = userInfos.value.signature;
	}
	clearSign();
	state.signDialogVisible = false;
};

// Revoke electronic signature
const unDoSign = () => {
	signaturePadRef.value.undoSignature();
};

// Clear electronic signature
const clearSign = () => {
	signaturePadRef.value.clearSignature();
};

// Upload handwritten electronic signature
const uploadSignFile = async (file: any) => {
	var res = await getAPI(SysFileApi).apiSysFileUploadSignaturePostForm(file.raw);
	userInfos.value.signature = res.data.result?.url;
	state.ruleFormBase.signature = userInfos.value.signature;
};

// Get a list of electronic signature files
const handleChangeSignFile = (_file: any, fileList: []) => {
	state.signFileList = fileList;
};

// Modify personal information
const submitUserBase = () => {
	ruleFormBaseRef.value?.validate(async (valid: boolean) => {
		if (!valid) return;
		ElMessageBox.confirm('Are you sure you want to modify your personal basic information?', 'Prompt', {
			confirmButtonText: 'Confirm',
			cancelButtonText: 'Cancel',
			type: 'warning',
		}).then(async () => {
			await getAPI(SysUserApi).apiSysUserBaseInfoPost(state.ruleFormBase);
			const accessToken = Local.get(accessTokenKey);
			await getAPI(SysAuthApi).apiSysAuthRefreshTokenGet(`${accessToken}`);
			window.location.reload();
		});
	});
};

// Password verification
const validatePassword = (_rule: any, value: any, callback: any) => {
	if (state.passwordNew2 != state.ruleFormPassword.passwordNew) {
		callback(new Error('The two passwords are inconsistent!'));
	} else {
		callback();
	}
};

// Password reset
const resetPassword = () => {
	state.ruleFormPassword.passwordOld = '';
	state.ruleFormPassword.passwordNew = '';
	state.passwordNew2 = '';
};

// Password submission
const submitPassword = () => {
	ruleFormPasswordRef.value?.validate(async (valid: boolean) => {
		if (!valid) return;

		// SM2 encryption password
		const cpwd: ChangePwdInput = { passwordOld: '', passwordNew: '' };
		const publicKey = window.__env__.VITE_SM_PUBLIC_KEY;
		cpwd.passwordOld = sm2.doEncrypt(state.ruleFormPassword.passwordOld, publicKey, 1);
		cpwd.passwordNew = sm2.doEncrypt(state.ruleFormPassword.passwordNew, publicKey, 1);
		await getAPI(SysUserApi).apiSysUserChangePwdPost(cpwd);

		// Exit the system
		ElMessageBox.confirm('The password has been changed. Do you want to log in to the system again?', 'Prompt', {
			confirmButtonText: 'Confirm',
			cancelButtonText: 'Cancel',
			type: 'warning',
		}).then(async () => {
			clearAccessAfterReload();
		});
	});
};

// Open the cropping pop-up window
const openCropperDialog = () => {
	state.cropperTitle = 'Change avatar';
	cropperDialogRef.value?.openDialog(userInfos.value.avatar);
};

// When the mouse enters and leaves the avatar
const mouseEnterAvatar = () => {
	state.avatarLoading = true;
};

const mouseLeaveAvatar = () => {
	state.avatarLoading = false;
};

// Executed when the number of uploaded signatures exceeds the limit
const uploadSignFileExceed: UploadProps['onExceed'] = (files) => {
	uploadSignRef.value!.clearFiles();
	const file = files[0] as UploadRawFile;
	file.uid = genFileId();
	uploadSignRef.value!.handleStart(file);
};

// Get file address
const getFileUrl = (row: SysFile): string => {
	if (row.bucketName == 'Local') {
		return `/${row.filePath}/${row.id}${row.suffix}`;
	} else {
		return row.url!;
	}
};

// Export object
defineExpose({ handleChangeSignFile });
</script>

<style lang="scss" scoped>
.account-center-avatarHolder {
	text-align: center;
	margin-bottom: 24px;

	.username {
		font-size: 20px;
		line-height: 28px;
		font-weight: 500;
		margin-bottom: 4px;
	}
}
.account-center-org {
	margin-bottom: 8px;
	position: relative;
	p {
		margin-top: 10px;
	}
	span {
		padding-left: 10px;
	}
}
.avatar {
	margin: 0 auto;
	width: 104px;
	height: 104px;
	margin-bottom: 20px;
	border-radius: 50%;
	overflow: hidden;
	img {
		height: 100%;
		width: 100%;
	}
}

.image-signature {
	margin-top: 20px;
	margin-bottom: 10px;
	width: 100%;
	height: 150px;
	background-color: #fff;
	text-align: center;
	vertical-align: middle;
	border: solid 1px var(--el-border-color);
}
</style>
