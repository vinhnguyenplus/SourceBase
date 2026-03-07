<template>
	<el-popover placement="bottom" width="280" trigger="hover" :show-after="600">
		<template #reference>
			<el-text type="primary" class="cursor-pointer">
				<el-icon><ele-InfoFilled /></el-icon>Details
			</el-text>
		</template>
		<el-descriptions direction="vertical" :column="2" border>
			<el-descriptions-item width="140">
				<template #label>
					<el-text>
						<el-icon><ele-UserFilled /></el-icon>Creator
					</el-text>
				</template>
				<el-tag>{{ props.data.createUserName ?? 'None' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-Calendar /></el-icon>Creation Time
					</el-text>
				</template>
				<el-tag>{{ props.data.createTime ?? 'None' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-UserFilled /></el-icon>Editor
					</el-text>
				</template>
				<el-tag>{{ props.data.updateUserName ?? 'None' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-Calendar /></el-icon>modification time
					</el-text>
				</template>
				<el-tag>{{ props.data.updateTime ?? 'None' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item v-if="'remark' in props.data">
				<template #label>
					<el-text>
						<el-icon><ele-Tickets /></el-icon>Remarks
					</el-text>
				</template>
				{{ props.data.remark ?? 'None' }}
			</el-descriptions-item>
		</el-descriptions>
	</el-popover>
</template>

<script lang="ts" setup>
const props = defineProps<{
	data: ModifyRecord;
}>();
</script>
