<template>
	<card-pro title="关于项目" prefix-icon="ele-QuestionFilled" shadow="hover">
        <p>
			Information!
		</p>
    </card-pro>
</template>

<script lang="ts">
export default {
	title: '关于项目',
	icon: 'ele-QuestionFilled',
	description: '点个星星支持一下',
};
</script>
<script setup lang="ts" name="about">
import CardPro from '/@/components/CardPro/index.vue';
</script>

<style scoped>
.item-background p {
	color: #999;
	margin-top: 10px;
	line-height: 1.8;
	/* height: 100px; */
}
</style>
