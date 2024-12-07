import { isFunction, isString } from "@Utilities/Methods";
import { TEXT, JSON_STRING } from "@Utilities/Constants";

const GET = 'GET';
const POST = 'POST';
const PUT = 'PUT';
const DELETE = 'DELETE';
const succsesCode = 200;

function makeRequest(method, url, data, successCallback, errorCallback, responseType, headers) {
    const args = {
        method: method,
        headers: headers,
    };

    if(data) {
        args.body = JSON.stringify(data);
    }

    fetch(url, args).then(response => {
        if(response.status !== succsesCode && isFunction(errorCallback)) {
            response?.json()?.then(responseData => {
                if(isString(responseData)) {
                    errorCallback(responseData);
                } else {
                    errorCallback(responseData?.errors[Object.keys(responseData.errors)[0]]);
                }
            });
        }
        else if(response.status === succsesCode && isFunction(successCallback)) {
            if(responseType === JSON_STRING) {
                response?.json()?.then(data => {
                    successCallback(data);
                })
            }
            else if(responseType === TEXT) {
                response?.text()?.then(data => {
                    successCallback(data);
                })
            } else if(!responseType) {
                successCallback(response?.data);
            }
        }
    }).catch(error => {
        if(isFunction(errorCallback)) {
            errorCallback(error?.response?.data);
        }
    });
}

export function get(url, data, successCallback, errorCallback, headers={}) {
    // if there is data, add it to the url
    if(data && Object.keys(data).length > 0) {
        url = url + '?';
        for(const [key, value] of Object.entries(data)) {
            url = url + `${key}=${value}&`;
        }
        url = url.slice(0, -1);
    }
    makeRequest(GET, url, null, successCallback, errorCallback, JSON_STRING, headers);
}

export function post(url, data, successCallback, errorCallback, responseType, headers={}) {
    makeRequest(POST, url, data, successCallback, errorCallback, responseType, headers);
}

export function put(url, data, successCallback, errorCallback, responseType, headers={}) {
    makeRequest(PUT, url, data, successCallback, errorCallback, responseType, headers);
}

export function del(url, data, successCallback, errorCallback, headers={}) {
    makeRequest(DELETE, url, data, successCallback, errorCallback, null ,headers);
}