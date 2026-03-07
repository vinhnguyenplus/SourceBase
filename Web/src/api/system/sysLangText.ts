import {useBaseApi} from '/@/api/base';

// Translation interface service
export const useSysLangTextApi = () => {
	const baseApi = useBaseApi("sysLangText");
	return {
		// Pagination query translation
		page: baseApi.page,
		// View translation details
		detail: baseApi.detail,
		// Add translation
		add: baseApi.add,
		// Update translation
		update: baseApi.update,
		// Delete translation
		delete: baseApi.delete,
		// Delete translations in batches
		batchDelete: baseApi.batchDelete,
		// Export translation data
		exportData: baseApi.exportData,
		// Import translation data
		importData: baseApi.importData,
		// Download translation data import template
		downloadTemplate: baseApi.downloadTemplate,
	}
}

// translation entity
export interface SysLangText {
	// Primary keyId
	id: number;
	// Name of the entity to which it belongs
	entityName?: string;
	// Owning entity ID
	entityId?: number;
	// Field name
	fieldName?: string;
	// language code
	langCode?: string;
	// Translate content
	content?: string;
	// creation time
	createTime: string;
	// Update time
	updateTime: string;
	// CreatorId
	createUserId: number;
	// Creator name
	createUserName: string;
	// Modifier ID
	updateUserId: number;
	// Modifier name
	updateUserName: string;
}