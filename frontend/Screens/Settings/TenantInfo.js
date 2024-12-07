import React from "react";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import { api } from '@Services/API';
import { translate } from "@Utilities/translate";

const TenantInfo = ({ tenant, handleError, handleSuccessEdit, mapFieldsToObject, token, isTenant, showLoader }) => {
    const handleTenantEdit = (fields) => {
        showLoader();
        const args = mapFieldsToObject(fields);
        api?.editTenant(
            args,
            token,
            handleSuccessEdit,
            handleError
        );
    };
    if(isTenant()) {
        return(
            <BoxInfo 
                key={`tenant-${tenant.id}`}
                fields={[
                    { 
                        icon: 'store',
                        label: `${translate["name_label"]}`,
                        text: tenant.name,
                        apiKey: 'name',
                    },
                    {
                        icon: 'envelope',
                        label: `${translate["email_label"]}`,
                        text: tenant.email,
                        editable: true,
                        editFunction: handleTenantEdit,
                        apiKey: 'email',
                    },
                    {
                        icon: 'phone',
                        label: `${translate["phone_label"]}`,
                        text: tenant.phoneNumber,
                        editable: true,
                        editFunction: handleTenantEdit,
                        apiKey: 'phoneNumber',
                    },
                    {
                        icon: 'location',
                        label: `${translate["address_label"]}`,
                        text: tenant.address,
                        editable: true,
                        editFunction: handleTenantEdit,
                        apiKey: 'address',
                    },
                ]}
            />
        );
    } else {
        return(
            <BoxInfo 
                key={`tenant-${tenant.id}`}
                fields={[
                    { 
                        icon: 'store',
                        text: `${translate["name_label"]} ${tenant.name}`
                    },
                    {
                        icon: 'envelope',
                        text: `${translate["email_label"]} ${tenant.email}`,
                    },
                    {
                        icon: 'phone',
                        text: `${translate["phone_label"]} ${tenant.phoneNumber}`,
                    },
                    {
                        icon: 'location',
                        text: `${translate["address_label"]} ${tenant.address}`,
                    },
                ]}
            />
        );
    }
};

export default TenantInfo;