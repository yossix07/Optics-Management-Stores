import React from 'react';
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import { api } from '@Services/API';
import { translate } from "@Utilities/translate";

const UserInfo = ({ user, handleError, handleSuccessEdit, mapFieldsToObject, token, showLoader }) => {
    const handleUserEdit = (fields) => {
        showLoader();
        const args = mapFieldsToObject(fields);
        api?.updateUser(
            args,
            token,
            handleSuccessEdit,
            handleError
        );
    };

    return(
        <BoxInfo 
            key={ `user-${user.id}` }
            fields={[
                { 
                    icon: 'id',
                    text: `${translate["id_label"]}${user.id}`,
                    apiKey: 'id'
                },
                { 
                    icon: 'person',
                    label: `${translate["name_label"]}`,
                    text: `${user.name}`,
                    editable: true,
                    editFunction: handleUserEdit,
                    apiKey: 'name'
                },
                { 
                    icon: 'envelope',
                    label: `${translate["email_label"]}`,
                    text: `${user.email}`,
                    editable: true,
                    editFunction: handleUserEdit,
                    apiKey: 'email'
                },
                { 
                    icon: 'phone',
                    label: `${translate["phone_label"]}`,
                    text: `${user.phoneNumber}`,
                    editable: true,
                    editFunction: handleUserEdit,
                    apiKey: 'phoneNumber'
                },
                {
                    icon: 'calendar',
                    label: `${translate["date_of_birth_label"]}`,
                    text: `${user.dateOfBirth.split('T')[0]}`,
                    editable: true,
                    editFunction: handleUserEdit,
                    apiKey: 'dateOfBirth'
                },
            ]}
        />
    );
};

export default UserInfo;