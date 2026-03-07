<template>
    <div class="sys-update-container">
        <!-- <div>
            <NoticeBar text="System update management, please operate with caution!" style="margin: 4px" />
        </div> -->
        <el-container>
            <el-aside v-auth="'sysUpdate:list'" width="220px" class="backup-list">
                <p class="backup-list-description">Backup list</p>
                <el-scrollbar>
                    <div class="backup-items">
                        <div v-for="(backup, index) in state.backups" :key="index" class="backup-item"
                            @mouseenter="hovered = index" @mouseleave="hovered = null">
                            <el-button type="text"
                                :class="{ 'selected-backup': state.selectedBackup === backup, 'hovered-backup': hovered === index }"
                                @click="() => state.selectedBackup = backup">
                                {{ backup.fileName }}
                            </el-button>
                        </div>
                    </div>
                </el-scrollbar>
            </el-aside>
            <el-main v-auth="'sysUpdate:logs'" class="log-terminal-container">
                <el-alert title="System update management, please operate with caution!" type="warning" show-icon />
                <div class="toolbar">
                    <el-button-group>
                        <el-button v-auth="'sysUpdate:update'" v-reclick="5000" :disabled="state.isUpdating"
                            @click="handleAction('update')">Update</el-button>
                        <el-button v-auth="'sysUpdate:restore'" v-reclick="5000"
                            :disabled="!canRestore || state.isUpdating || !state.selectedBackup"
                            @click="handleAction('restore')">restore</el-button>
                        <el-button v-auth="'sysUpdate:clear'" v-reclick="5000" :disabled="state.isUpdating"
                            @click="clearLogs">Clear</el-button>
                        <el-button v-auth="'sysUpdate:webHookKey'" v-reclick="5000"
                            @click="getWebHookKey">Get key</el-button>
                    </el-button-group>
                </div>
                <div class="log-terminal">
                    <div class="terminal-output" ref="terminalOutput">
                        <pre>{{ state.logOutput }}</pre>
                    </div>
                </div>
            </el-main>
        </el-container>
    </div>
</template>

<script setup lang="ts">
import { reactive, computed, onMounted, nextTick, ref, onUnmounted } from 'vue';
import { ElMessage, ElMessageBox } from "element-plus";
import { getAPI } from "/@/utils/axios-utils";
import { authAll } from "/@/utils/authFunction";
import { BackupOutput, SysUpdateApi } from "/@/api-services";
import commonFunction from "/@/utils/commonFunction";
import NoticeBar from "/@/components/noticeBar/index.vue";

const { copyText } = commonFunction();
const state = reactive({
    selectedBackup: null as BackupOutput | null,
    backups: [] as BackupOutput[],
    isUpdating: false,
    logOutput: '',
});

// Computed property canRestore
const canRestore = computed(() => !!state.selectedBackup);

// reference element
const terminalOutput = ref<HTMLElement | null>(null);

// New hover index variable
const hovered = ref<number | null>(null);

let refreshInterval: number;

// Get initial data
const fetchData = async () => {
    try {
        state.backups = (await getAPI(SysUpdateApi).apiSysUpdateListPost()).data.result ?? [];
        await refreshLog();
    } catch (error) {
        handleError('Failed to retrieve data', error);
    }
};

// Refresh log
const refreshLog = async () => {
    try {
        const response = await getAPI(SysUpdateApi).apiSysUpdateLogsGet();
        state.logOutput = (response.data.result ?? []).join('\n');
        scrollToBottom(); // Scroll to bottom immediately after update log
    } catch (error) {
        handleError('Failed to get log', error);
    }
};

// scroll to bottom
const scrollToBottom = () => {
    nextTick(() => {
        if (terminalOutput.value) {
            terminalOutput.value.scrollTop = terminalOutput.value.scrollHeight;
        }
    });
};

// Start/stop log refresh timer
const toggleRefreshTimer = (start: boolean) => {
    if (start && !refreshInterval) {
        refreshInterval = window.setInterval(refreshLog, 300);
    } else if (!start && refreshInterval) {
        window.clearInterval(refreshInterval);
        refreshInterval = 0;
    }
};

// processing action
const handleAction = async (action: 'update' | 'restore') => {
    if (state.isUpdating) return;

    state.isUpdating = true;
    toggleRefreshTimer(true);

    try {
        switch (action) {
            case 'update':
                await getAPI(SysUpdateApi).apiSysUpdateUpdatePost({ timeout: -1 });
                ElMessage.success('Update successful');
                fetchData();
                break;
            case 'restore':
                ElMessageBox.confirm(`Are you sure you want to restore to ${state.selectedBackup?.fileName}?`, 'Prompt', {
                    confirmButtonText: 'Confirm',
                    cancelButtonText: 'Cancel',
                    type: 'warning',
                }).then(async () => {
                    await getAPI(SysUpdateApi).apiSysUpdateRestorePost({ fileName: state.selectedBackup?.fileName } as any);
                    ElMessage.success('Restore successful');
                });
                break;
        }
    } catch (error) {
        handleError(`Failed to execute ${action}`, error);
    } finally {
        toggleRefreshTimer(false);
        state.isUpdating = false;
    }
};

// Clear log
const clearLogs = async () => {
    try {
        state.logOutput = '';
        await getAPI(SysUpdateApi).apiSysUpdateClearGet();
        ElMessage.success('The log has been cleared');
    } catch (error) {
        handleError('Failed to clear logs', error);
    }
};

// Get key
const getWebHookKey = async () => {
    try {
        const res = await getAPI(SysUpdateApi).apiSysUpdateWebHookKeyGet();
        if (res.data.result) copyText(res.data.result);
    } catch (error) {
        handleError('Failed to obtain key', error);
    }
}

// Error handling
const handleError = (message: string, error: any) => {
    ElMessage.error(`${message}, please try again later.`);
};

onMounted(() => {
    if (!authAll(['sysUpdate:list', 'sysUpdate:logs'])) return;
    fetchData();
    toggleRefreshTimer(true);
});

onUnmounted(() => {
    if (!authAll(['sysUpdate:list', 'sysUpdate:logs'])) return;
    toggleRefreshTimer(false);
});
</script>

<style scoped>
.sys-update-container {
    display: flex;
    height: 100%;
    background-color: var(--next-bg-main-color);
}

.backup-list-description {
    margin-bottom: 10px;
    color: #909399;
}

.backup-list {
    background-color: var(--el-bg-color);
    padding: 20px;
    /* box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); */
    border-radius: 4px;
    transition: box-shadow 0.3s ease-in-out;
    max-height: 100%;
    overflow: hidden;
    border: var(--el-border);
}

.backup-items {
    max-height: calc(100vh - 40px);
    overflow-y: auto;
    overflow-x: hidden;
}

.backup-item {
    margin-bottom: 10px;
    transition: transform 0.2s;
}

.backup-item:hover {
    transform: translateX(5px);
}

.selected-backup,
.hovered-backup {
    font-weight: bold;
    color: #409eff;
}

.action-button {
    margin-top: 8px;
    transition: background-color 0.3s;
}

.action-button:hover {
    background-color: #ecf5ff;
}

.log-terminal-container {
    flex-grow: 1;
    padding: 20px;
    margin-left: 5px;
    display: flex;
    flex-direction: column;
    background-color: var(--el-bg-color);
    border-radius: 4px;
    /* box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); */
    transition: box-shadow 0.3s ease-in-out;
    border: var(--el-border);
}

.toolbar {
    /* margin-bottom: 5px; */
    padding: 5px 0;
    background-color: var(--el-bg-color);
    border-radius: 4px;
    /* box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1); */
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: flex-start;
    gap: 8px;
}

.log-terminal {
    background-color: #2c3e50;
    color: #ecf0f1;
    border-radius: 4px;
    flex-grow: 1;
    position: relative;
    overflow: hidden;
    /* box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); */
}

.terminal-output {
    padding: 20px;
    height: 100%;
    overflow-y: auto;
    white-space: pre-wrap;
    word-wrap: break-word;
    font-family: 'Courier New', Courier, monospace;
    font-size: 14px;
    line-height: 1.5;
}

.el-alert {
	border: none;
}
</style>