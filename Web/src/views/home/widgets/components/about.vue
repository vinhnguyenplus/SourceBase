<template>
	<card-pro title="About the project" prefix-icon="ele-QuestionFilled" shadow="hover">
        <p>
			Information!
		</p>
    </card-pro>
</template>

<script lang="ts">
export default {
	title: 'About the project',
	icon: 'ele-QuestionFilled',
	description: 'Give a star to show support',
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
