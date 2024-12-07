import React, { createContext, useState } from 'react';
import { USER, TENANT } from '@Utilities/Constants';

export const UserContext = createContext();

export function UserProvider({ children }) {
  const [ isLoggedIn, setIsLoggedIn ] = useState(false);
  const [ username, setUsername ] = useState('');
  const [ token, setToken ] = useState('');
  const [ permissions, setPermissions ] = useState('');

  const isTenant = () => {
    return permissions === TENANT;
  }

  const isUser = () => {
    return permissions === USER;
  }

  const logInFunction = (username, token, perrmisions) => {
    setUsername(username);
    setToken(token);
    setPermissions(perrmisions);
    setIsLoggedIn(true);
  };

  const logOutFunction = () => {
    setUsername('');
    setToken('');
    setPermissions('');
    setIsLoggedIn(false);
  };

  return (
    <UserContext.Provider value={{ username, isTenant, isUser, isLoggedIn, token, logInFunction, logOutFunction }}>
      { children }
    </UserContext.Provider>
  ); 
};