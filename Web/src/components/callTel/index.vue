<template>
	<div class="page-container">
		<!-- name and number part -->
		<div class="name-number" v-if="state.data">
			<span class="name">{{ state.data.realName }}</span>
			<span class="spacing"></span> <!-- Add NewspacingYuanplain -->
			<span class="number">{{ state.data.number }}</span>
		</div>
		<!-- avatar container -->
		<div class="avatars">
			<!-- green avatar -->
			<div class="avatar-wrap green-icon-wrap" @click="handleClick(true)" v-reclick="1000">
				<el-avatar :size="100" class="icon green-icon">
					<i class="iconfont icon-dianhua"></i>
				</el-avatar>
			</div>
		</div>
	</div>
</template>

<script lang="ts" name="callTel" setup>
import { reactive, onMounted } from "vue";
import { useRoute } from "vue-router";
const route = useRoute();

const state = reactive({
	token: null as any,
	data: {} as any
});

// Page initialization
const initializePage = () => {
	state.data = JSON.parse(decodeURIComponent(route.query.code || '{}' as any));
	state.token = route.query.token;
};

onMounted(() => {
	initializePage();
});

// click event
const handleClick = (success: boolean) => {
	if (success) location.href = 'tel:' + state.data.number;
};
</script>

<style lang="scss" scoped>
/* style remains the same */
.page-container {
	display: flex;
	flex-direction: column;
	justify-content: flex-start; /* Useflex-startbut notYescenterto controlverticalAlign */
	align-items: center; /* levelcenter */
	height: 100vh;
	padding-top: calc(100vh * (1 - 1/1.618)); /* Use goldpointsCalculate top padding by split ratio */
}

.name-number {
	text-align: center;
	margin-bottom: 20px; /* Add toandAvatarspacing between */
	.name {
		font-weight: bold;
		font-size: 24px; /* Increase font size */
	}

	.spacing {
		display: block;
		height: 10px; /* Set the height of the spacing */
	}

	.number {
		font-size: 20px; /* Increase font size */
		color: #a09e9e; /* Change the number color to the specified light gray */
		font-weight: 600; /* Bold font */
	}
}

.avatars {
	display: flex;
	justify-content: center; /* makechildYuanplain（GreenAvatar）levelcenter */
	gap: 40px; /* increaseAvatarspacing between */
	.avatar-wrap {
		cursor: pointer;
		transition: background-color 0.2s, transform 0.2s;
	}

	.avatar-wrap:hover .icon {
		filter: brightness(90%); /* ratLabel hovertimeSlightly darken */
	}

	.avatar-wrap:active .icon {
		filter: brightness(80%); /* ClicktimeBecome darker more noticeably */
		transform: scale(0.94); /* ClicktimeSlightReduce */
	}

	.icon {
		color: white;
		transition: filter 0.2s, transform 0.2s;
	}

	.iconfont {
		font-size: 70px;
	}

	.green-icon {
		background-color: lawngreen;
	}

	.red-icon {
		background-color: red;
	}

	.green-icon-wrap .icon {
		background-color: lawngreen;
	}

	.red-icon-wrap .icon {
		background-color: red;
	}
}
</style>