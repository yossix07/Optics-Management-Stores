import { get, post, put, del } from "@Utilities/fetch";
import { translate } from "@Utilities/translate";
import { baseUrl, tenantID } from "../env";
import { TEXT, JSON_STRING } from "@Utilities/Constants";

export const api = {
    // Auth
    userLogin: userLogin,
    userSignup: userSignup,
    tenantLogin: tenantLogin,
    sendResetPasswordEmail: sendResetPasswordEmail,
    resetPassword: resetPassword,
    
    // Users
    getUserById: getUserById,
    updateUser: updateUser,

    // Tenants
    getTenantById: getTenantById,
    editTenant: editTenant,

    // Appointments
    getAvailableAppointments: getAvailableAppointments,
    bookAppointment: bookAppointment,
    getAppointmentsByUserId: getAppointmentsByUserId,
    getBookedAppointments: getBookedAppointments,
    removeAppointment: removeAppointment,

    // Appointments Settings
    getAppointmentsTypes: getAppointmentsTypes,
    addAppointmentsType: addAppointmentsType,
    removeAppointmentsType: removeAppointmentsType,
    editAppointmentsType: editAppointmentsType,
    getAvaliableBlocks: getAvaliableBlocks,
    addAvaliableBlock: addAvaliableBlock,
    removeAvaliableBlock: removeAvaliableBlock,
    getDaysOff: getDaysOff,
    addDayOff: addDayOff,
    removeDayOff: removeDayOff,
    getSlotDuration: getSlotDuration,
    editSlotDuration: editSlotDuration,
    addCustomAppointmentSlot: addCustomAppointmentSlot,

    // Store
    addProduct: addProduct,
    removeProduct: removeProduct,
    editProduct: editProduct,
    getProductById: getProductById,
    getAllProducts: getAllProducts,

    // Statistics
    getGeneralStats: getGeneralStats,
    getProductSales: getProductSales,
    getProductIncome: getProductIncome,
    getProductAmount: getProductAmount,
    getProductsIncome: getProductsIncome,
    getOrdersAmount: getOrdersAmount,
    getOrdersIncome: getOrdersIncome,

    // Orders
    getOrdersByStatus: getOrdersByStatus,
    createOrder: createOrder,
    updateOrderStatus: updateOrderStatus,
    getOrdersByUserId: getOrdersByUserId,
}

// Auth

function userLogin(id, password, successCallback, errorCallback) {
    post(
        baseUrl + `/api/Auth/login/${tenantID}/user`,
        { 
            "id": id,
            "password": password
        },
        successCallback,
        errorCallback,
        'json',
        {
            'Content-Type' : 'application/json',
            'Accept': 'application/json',
        }
    );
}

function userSignup(id, name, password, email, phoneNumber, dateOfBirth, successCallback, errorCallback) {
    post(
        baseUrl + `/api/Auth/register/${tenantID}/user`,
        {
            "id": id,
            "name": name,
            "password": password,
            "email": email,
            "phoneNumber": phoneNumber,
            "dateOfBirth": dateOfBirth
        },
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            'Accept': 'application/json'
        }
    );
}

function tenantLogin(name, password, successCallback, errorCallback) {
    post(
        baseUrl + `/api/Auth/login/tenant`,
        { 
            "name": name,
            "password": password,
            tenantId: tenantID
        },
        successCallback,
        errorCallback,
        'json',
        {
            'Content-Type' : 'application/json',
            'Accept': 'application/json',
        }
    );
}

function sendResetPasswordEmail(email, role, successCallback, errorCallback) {
    post(
        baseUrl + `/api/Auth/resetPassword/${tenantID}`,
        {
            "email": email,
            "role": role
        },
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            'Accept': 'application/json'
        }
    );
};

function resetPassword(formData, successCallback, errorCallback) {
    if (formData.newPassword !== formData.passwordValidation) {
        errorCallback(translate('passwordsDontMatch'));
        return;
    }
    post(
        baseUrl + `/api/Auth/verifyCode/${tenantID}`,
        formData,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            'Accept': 'application/json'
        }
    );
}

// Users

function getUserById(userId, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/User/${userId}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function updateUser(user, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/${tenantID}/User/${user.id}`,
        user,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

// Tenants
function getTenantById(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/Tenant/${tenantID}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function editTenant(tenant, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/Tenant/${tenantID}`,
        tenant,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

// Appointments

function getAvailableAppointments(startDate, endDate, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Appointments/GetAvailable`,
        {
            "Start.Year": startDate?.year,
            "Start.Month": startDate?.month,
            "Start.Day": startDate?.day,
            "End.Year": endDate?.year,
            "End.Month": endDate?.month,
            "End.Day": endDate?.day
        },
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function bookAppointment(appointmentId, userId, typeName, description, token, successCallback, errorCallback) {
    if(!typeName) {
        errorCallback(translate["missing_appointment_type"]);
        return;
    }
    post(
        baseUrl + `/api/${tenantID}/Appointments`,
        {
            "appointmentId": appointmentId,
            "userId": userId,
            "typeName": typeName,
            "description": description,
        },
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getAppointmentsByUserId(userId, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Appointments/${userId}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getBookedAppointments(startDate, endDate, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Appointments/GetByStatus`,
        {
            "Start.Year": startDate?.year,
            "Start.Month": startDate?.month,
            "Start.Day": startDate?.day,
            "Status": 1,
            "End.Year": endDate?.year,
            "End.Month": endDate?.month,
            "End.Day": endDate?.day
        },
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function removeAppointment(appointmentId, username, token, successCallback, errorCallback) {
    del(
        baseUrl + `/api/${tenantID}/Appointments/${appointmentId}`,
        username,
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}


// Appointments Settings
function getAppointmentsTypes(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/AppointmentSettings/Types/GetAll`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
        );
}

function addAppointmentsType(type, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/AppointmentSettings/Type/Create`,
        type,
        successCallback,
        errorCallback,
        'json',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function removeAppointmentsType(typeName, token, successCallback, errorCallback) {
    del(
        baseUrl + `/api/${tenantID}/AppointmentSettings/Type/Delete`,
        typeName,
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function editAppointmentsType(type, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/${tenantID}/AppointmentSettings/Type/Update`,
        type,
        successCallback,
        errorCallback,
        'json',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getAvaliableBlocks(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/AppointmentSettings/AvailableBlocks/GetAll`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
};

function addAvaliableBlock(block, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/AppointmentSettings/AvailableBlocks/Create`,
        block,
        successCallback,
        errorCallback,
        'json',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function removeAvaliableBlock(block, token, successCallback, errorCallback) {
    del(
        baseUrl + `/api/${tenantID}/AppointmentSettings/AvailableBlocks/Delete`,
        block,
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getDaysOff(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/AppointmentSettings/DayOff/GetAll`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function addDayOff(dayOff, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/AppointmentSettings/DayOff/Create`,
        dayOff,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function removeDayOff(dayOff, token, successCallback, errorCallback) {
    del(
        baseUrl + `/api/${tenantID}/AppointmentSettings/DayOff/Delete`,
        dayOff,
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getSlotDuration(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/AppointmentSettings/SlotDuration/Get`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function editSlotDuration(slotDuration, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/${tenantID}/AppointmentSettings/SlotDuration/Update`,
        slotDuration,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function addCustomAppointmentSlot(slot, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Appointments/CreateCustomAppointment`,
        slot,
        successCallback,
        errorCallback,
        'text',
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

// Store
function addProduct(product, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Product`,
        product,
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function removeProduct(productId, token, successCallback, errorCallback) {
    del(
        baseUrl + `/api/${tenantID}/Product/delete/${productId}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function editProduct(product, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/${tenantID}/Product/put/${product.id}`,
        {
            name: product.name,
            description: product.description,
            price: product.price,
            stock: product.stock,
            image: product.image
        },
        successCallback,
        errorCallback,
        TEXT,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );

}

function getProductById(productId, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Product/get/${productId}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getAllProducts(token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Product`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}


// Statistics

function getGeneralStats(startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/generalStatsticByDate`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getProductSales(productId, startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/product/amount/${productId}`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getProductIncome(productId, startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/product/money/${productId}`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getProductAmount(startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/productStatistics/amout`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
};

function getProductsIncome(startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/productStatistics/price`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
};

function getOrdersAmount(startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/orderStatistics/amount`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
};

function getOrdersIncome(startDate, endDate, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Statistics/orderStatistics/money`,
        {
            start:{ ...startDate },
            end:{ ...endDate }
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
};
// Orders
function getOrdersByStatus(status, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Order/getAll/${status}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function createOrder(userId, cartItems, token, successCallback, errorCallback) {
    post(
        baseUrl + `/api/${tenantID}/Order`,
        {
            userId: userId,
            cartItems: cartItems
        },
        successCallback,
        errorCallback,
        JSON_STRING,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function updateOrderStatus(orderId, status, token, successCallback, errorCallback) {
    put(
        baseUrl + `/api/${tenantID}/Order/update/orderStatus`,
        {
            orderId: orderId,
            status: status
        },
        successCallback,
        errorCallback,
        TEXT,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}

function getOrdersByUserId(userId, status, token, successCallback, errorCallback) {
    get(
        baseUrl + `/api/${tenantID}/Order/getByUser/${userId}/${status}`,
        {},
        successCallback,
        errorCallback,
        {
            'Content-Type' : 'application/json',
            Authorization: `Bearer ${token}`
        }
    );
}