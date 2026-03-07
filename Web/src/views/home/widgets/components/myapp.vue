<template>
	<el-card shadow="hover" header="Quick Access" body-style="padding: 0">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Guide /> </el-icon>
			<span> Quick Access </span>
		</template>
		<ul class="myMods">
			<li v-for="mod in myMods" :key="mod.path!">
				<router-link :to="{ path: mod.path! }">
					<SvgIcon :name="mod.meta?.icon" style="font-size: 18px" />
					<p>{{ mod.meta?.title }}</p>
				</router-link>
			</li>
			<li class="modItem-add" @click="addMods">
				<a>
					<el-icon><ele-Plus :style="{ color: '#fff' }" /></el-icon>
				</a>
			</li>
		</ul>

		<el-drawer title="Add app" v-model="modsDrawer" :size="520" destroy-on-close :before-close="beforeClose">
			<div class="setMods mt15">
				<h4>My favorite ( {{ myMods.length }} )</h4>
				<VueDraggable tag="ul" v-model="myMods" :animation="200" group="app" class="draggable-box">
					<li v-for="item in myMods" :key="item.id">
						<SvgIcon :name="item.meta?.icon" style="font-size: 18px" />
						<p>{{ item.meta?.title }}</p>
					</li>
				</VueDraggable>
			</div>
			<div class="setMods">
				<h4>All apps ( {{ filterMods.length }} )</h4>
				<VueDraggable tag="ul" v-model="filterMods" :animation="200" group="app" class="draggable-box-all">
					<li v-for="item in filterMods" :key="item.id" :style="{ background: '#909399' }">
						<SvgIcon :name="item.meta?.icon" style="font-size: 18px" />
						<p>{{ item.meta?.title }}</p>
					</li>
				</VueDraggable>
			</div>
			<template #footer>
				<div style="margin: 0 20px 20px 0">
					<el-button @click="beforeClose">Cancel</el-button>
					<el-button type="primary" @click="saveMods">save</el-button>
				</div>
			</template>
		</el-drawer>
	</el-card>
</template>

<script lang="ts">
export default {
	title: 'Quick Access',
	icon: 'ele-Guide',
	description: 'OkayConfigurationofQuick Access',
};
</script>

<script setup lang="ts" name="myapp">
import { onMounted, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { VueDraggable } from 'vue-draggable-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysUserMenuApi } from '/@/api-services/api';
import { MenuOutput } from '/@/api-services/models';
import { useRequestOldRoutes } from '/@/stores/requestOldRoutes';

const mods = ref<MenuOutput[]>([]); // All apps
const myMods = ref<MenuOutput[]>([]); // My usual
const myModsName = ref<Array<string | null | undefined>>([]); // My usual
const filterMods = ref<MenuOutput[]>([]); // Filter my favorite apps
const modsDrawer = ref<boolean>(false);

onMounted(() => {
	getMods();
});

// Request a list of favorited menus
const getFavoriteMenuList = async () => {
	try {
		const res = await getAPI(SysUserMenuApi).apiSysUserMenuUserMenuListGet();
		return res.data.result || [];
	} catch (error) {
		return [];
	}
};

const addMods = () => {
	modsDrawer.value = true;
};

const getMods = async () => {
	var menuTree = (useRequestOldRoutes().requestOldRoutes as MenuOutput[]) || [];
	filterMenu(menuTree);

	myMods.value = await getFavoriteMenuList();

	myModsName.value = myMods.value.map((v: MenuOutput) => v.name);
	filterMods.value = mods.value.filter((item: MenuOutput) => {
		return !myModsName.value.includes(item.name);
	});
};

// Recursively get all the second-level menus that can display non-iframes
const filterMenu = (map: MenuOutput[]) => {
	map.forEach((item: MenuOutput) => {
		if (item.meta?.isHide || item.type == 3 || item.status != 1) {
			return false;
		}
		if (item.meta?.isIframe) {
			item.path = `/i/${item.name}`;
		}
		if (item.children && item.children.length > 0) {
			filterMenu(item.children);
		} else {
			mods.value.push(item);
		}
	});
};

// Save my favorites
const saveMods = async () => {
	const menuIds = myMods.value.map((v: MenuOutput) => v.id) as any;
	await getAPI(SysUserMenuApi).apiSysUserMenuAddPost({ menuIdList: menuIds });
	ElMessage.success('Setting up common success');
	modsDrawer.value = false;
};

// Cancel
const beforeClose = async () => {
	myMods.value = await getFavoriteMenuList();
	myModsName.value = myMods.value.map((v: MenuOutput) => v.name);
	filterMods.value = mods.value.filter((item: MenuOutput) => {
		return !myModsName.value.includes(item.name);
	});
	modsDrawer.value = false;
};
</script>

<style scoped lang="scss">
.myMods {
	list-style: none;
}
.myMods li {
	display: inline-block;
	width: 100px;
	height: 100px;
	vertical-align: top;
	transition: all 0.3s ease;
	margin: 10px;
	border-radius: 5px;
	background: var(--el-color-primary);
}
.myMods li:hover {
	opacity: 0.5;
}
.myMods li a {
	width: 100%;
	height: 100%;
	padding: 10px;
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	text-align: center;
	color: #fff;
}
.myMods li i {
	font-size: 26px;
	color: #fff;
}
.myMods li p {
	font-size: 14px;
	color: #fff;
	margin-top: 10px;
	width: 100%;
	white-space: nowrap;
	text-overflow: ellipsis;
	overflow: hidden;
}

.modItem-add {
	border: 1px dashed #ddd;
	cursor: pointer;
}
.modItem-add i {
	font-size: 30px;
	color: #999 !important;
}
.modItem-add:hover,
.modItem-add:hover i {
	border-color: #409eff;
	color: #409eff !important;
}

.draggable-box {
	border: 1px dashed var(--el-color-primary);
	padding: 15px;
}

.draggable-box-all {
	border: 1px dashed var(--el-color-primary);
	padding: 15px;
	height: calc(100vh - 330px);
	overflow-y: scroll;
}

.draggable-box-all::-webkit-scrollbar {
	display: none;
}

.setMods {
	padding: 0 20px;
}
.setMods h4 {
	font-size: 14px;
	font-weight: normal;
}
.setMods ul {
	margin: 20px -5px;
	min-height: 90px;
}
.setMods li {
	display: inline-block;
	width: 80px;
	height: 80px;
	text-align: center;
	margin: 5px;
	color: #fff;
	vertical-align: top;
	padding: 4px;
	padding-top: 15px;
	cursor: move;
	border-radius: 3px;
	background: var(--el-color-primary);
}
.setMods li i {
	font-size: 20px;
}
.setMods li p {
	font-size: 12px;
	margin-top: 10px;
}
.setMods li.sortable-ghost {
	opacity: 0.3;
}
a {
	text-decoration: none;
}
</style>
