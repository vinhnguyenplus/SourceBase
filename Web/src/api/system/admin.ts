import request from '/@/utils/request';
enum Api {
	DictTypeDataList = '/api/sysDictData/DataList',
	AllDictList = '/api/sysDictType/AllDictList',
	HardwareInfo = '/api/sysServer/hardwareInfo',
	RuntimeInfo = '/api/sysServer/runtimeInfo',
	NuGetPackagesInfo = '/api/sysServer/nuGetPackagesInfo',
}

// Get a collection of dictionary values ​​based on dictionary type encoding
export const getDictDataList = (params?: any) =>
	request({
		url: `${Api.DictTypeDataList}/${params}`,
		method: 'get',
	});

// Get all dictionaries
export const getAllDictList = () =>
	request({
		url: `${Api.AllDictList}`,
		method: 'get',
	});

	// Get hardware information
export const getHardwareInfo = () =>
	request({
		url: `${Api.HardwareInfo}`,
		method: 'post',
	});

// Get runtime information
export const getRuntimeInfo = () =>
	request({
		url: `${Api.RuntimeInfo}`,
		method: 'post',
	});

// Get NuGet package information
export const getNuGetPackagesInfo = () =>
	request({
		url: `${Api.NuGetPackagesInfo}`,
		method: 'post',
	});

