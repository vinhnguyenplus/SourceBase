<template>
	<div class="el-card box">
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Postcard /> </el-icon>About
			</h4>
			<span class="text">
				Based on .NET6 (Furion/SqlSugar) Generic permission development framework，The frontend uses
				Vue3+Element-plus+Vite5，wholeCombining many excellent technologies and frameworks，Modular Plugin-Based Development。Highly integratedtenant、cache、DataVerification、Authentication、Event Bus、DynamicAPI、Communication、Remote request、Task Scheduling、Printand many other high-tech innovations。The code structure is simple and clear，Detailed annotations，Easy to get started withTwoSecondary development，Even ifYesComplex business logic can also be implemented quickly，Truejustachieve“Ready to use out of the box”。
			</span>
		</div>
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Warning /> </el-icon>Project information (Information)
			</h4>
			<el-descriptions :column="2" border>
				<el-descriptions-item label="Name and Author">
					<el-tag>{{ name }}</el-tag> <el-tag type="info">{{ author }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="Framework Description">
					<el-tag>{{ description }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="version number">
					<el-tag>{{ version }}</el-tag> <el-tag type="success">{{ license }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="Release Time">
					<el-tag>{{ lastBuildTime }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="Gitee">
					<el-link type="primary" href="https://gitee.com/zuohuaijun/Admin.NET" target="_blank"> Gitee </el-link>
				</el-descriptions-item>
				<el-descriptions-item label="Github">
					<el-link type="primary" href="https://github.com/zuohuaijun/Admin.NET.git" target="_blank"> Github </el-link>
				</el-descriptions-item>

				<el-descriptions-item label="Document address">
					<el-link type="primary" href="http:// 101.43.53.74:5050/" target="_blank"> Document address </el-link>
				</el-descriptions-item>
				<el-descriptions-item label="Preview address">
					<el-link type="primary" href="http:// 101.43.53.74:5005/dist/index.html" target="_blank"> Preview address </el-link>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>Production environment dependencies (Dependencies)
			</h4>
			<el-descriptions :column="3" border>
				<el-descriptions-item v-for="(value, key) in dependencies" :key="key" width="400px" :label="key">
					<el-tag type="success" effect="plain">
						{{ value }}
					</el-tag>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>Development environment dependencies (devDependencies)
			</h4>
			<el-descriptions :column="3" border>
				<el-descriptions-item v-for="(value, key) in devDependencies" :key="key" width="400px" :label="key">
					<el-tag type="danger" effect="plain">
						{{ value }}
					</el-tag>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>Keywords
			</h4>
			<el-descriptions :column="4" border>
				<el-descriptions-item v-for="(value, key) in keywords" :key="value" width="400px" :label="key + 1">
					<el-text type="primary">
						{{ value }}
					</el-text>
				</el-descriptions-item>
			</el-descriptions>
		</div>
	</div>
</template>

<script setup lang="ts" name="about">
import PackageJson from '/package.json';

const { dependencies, devDependencies, keywords, version, lastBuildTime, author, description, license, name } = PackageJson;
</script>

<style lang="scss" scoped>
.box {
	overflow-y: auto;
}
el-descriptions-item {
	width: 50%;
}
.card {
	padding: 10px;
	.title {
		margin: 5px 5px 10px;
		font-size: 17px;
		font-weight: bold;
		color: var(--el-text-color-primary);
	}
	.text {
		text-indent: 50px;
		font-size: 15px;
		line-height: 30px;
		padding: 10px 20px;
		color: var(--el-text-color-regular);
		.el-link {
			font-size: 15px;
		}
	}
}
</style>
