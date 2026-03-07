<template>
	<div class="sys-open-access-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-QuestionFilled /> </el-icon>
					<span> illustrate </span>
				</div>
			</template>
			<div class="text-content">
				<h2>Using OpenAPI</h2>
				<ul>
					<li>
						Paste in APIs that require Signature authentication
						<p><el-tag>[Authorize(AuthenticationSchemes = SignatureAuthenticationDefaults.AuthenticationScheme)]</el-tag></p>
					</li>
					<li>
						If the API needs to retain Jwt authentication, you can paste
						<p><el-tag>[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + SignatureAuthenticationDefaults.AuthenticationScheme)]</el-tag></p>
					</li>
					<li>
						By signing the request, the following objectives can be achieved:
						<ul>
							<li>ExemptLoginRecognition Access InterfaceUseridentity</li>
							<li>Prevent potential replay attacks</li>
						</ul>
					</li>
				</ul>
				<el-divider />
				<h2>OpenAPI Signature Process</h2>
				When the client makes a request, it needs to generate a signature and add public parameters according to the following steps:
				<h3>Public Request Parameters</h3>
				<p>Add Header request parameters based on the original request</p>
				<ul>
					<li><el-tag effect="plain">accessKey</el-tag>: Identity Identifier</li>
					<li><el-tag effect="plain">timestamp</el-tag>: Timestamp, accurate to the second</li>
					<li><el-tag effect="plain">nonce</el-tag>: A unique random number, it is recommended to be a 6-digit random number</li>
					<li><el-tag effect="plain">sign</el-tag>:Signature data (see "Computing Signatures" section)</li>
				</ul>
				<h3>Compute Signature</h3>
				<ul>
					<li>
						Sort the parameters in the request in the following order, and concatenate each parameter with & (without spaces in the middle):
						<p><el-tag>method & url & accessKey & timestamp & nonce</el-tag></p>
						<ul>
							<li><el-tag effect="plain">method</el-tag> Capital letters are required, such as: GET</li>
							<li><el-tag effect="plain">url</el-tag> Remove the protocol, domain, and parameters, starting with /, for example: /api/demo/helloWord</li>
						</ul>
					</li>
					<li>Create a hash-based message authentication code (HMAC) using the HMAC-SHA256 protocol to <el-tag effect="plain">accessSecret</el-tag> As a key, calculate the signature on the parameters spliced above, and the resulting signature is Base-64 encoded.</li>
				</ul>
			</div>
			<div class="el-alert el-alert--info is-light">
				HMAC-SHA256 online calculation:
				<el-link href="https://1024tools.com/hmac" target="_blank" type="primary">https://1024tools.com/hmac</el-link>
			</div>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysOpenAccessHelpView">
import { reactive } from 'vue';

const state = reactive({
	isShowDialog: false,
});

// Open pop-up window
const openDialog = () => {
	state.isShowDialog = true;
};

// // closure
// const close = () => {
// 	state.isShowDialog = false;
// };

// Export object
defineExpose({ openDialog });
</script>
<style scoped lang="scss">
.text-content {
	h1 {
		margin: 8px 0;
	}
	h2 {
		margin: 8px 0;
	}
	h3 {
		margin: 8px 0;
	}
	p {
		margin: 8px 0;
	}
	ul {
		padding: 0 0 0 30px;
		li {
			margin: 8px 0;
		}
	}
}
</style>
