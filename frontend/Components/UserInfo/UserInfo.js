import React from "react";
import { translate } from "@Utilities/translate";
import BoxInfo from "@Components/BoxInfo/BoxInfo";

const UserInfo = ({ user }) => {
    return(
        <BoxInfo fields={[
            {
                icon: 'user',
                text: `${translate['name_placeholder']} : ${user.name}`,
            },
            {
                icon: 'envelope',
                text: `${translate['email_placeholder']} : ${user.email}`,
            },
            {
                icon: 'phone',
                text: `${translate['phone_placeholder']} : ${user.phone}`,
            },
        ]}/>
    );
};

export default UserInfo;