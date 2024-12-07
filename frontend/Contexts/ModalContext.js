import React, { createContext, useState } from 'react';
import ConfirmModal from '@Components/ConfirmModal/ConfirmModal';
import { translate } from '@Utilities/translate';

export const ModalContext = createContext();

export function ModalProvider({ children }) {
  const [modalData, setModalData] = useState({
    isModalVisible: false,
    onApprove: null,
    approveText: null,
    onCancel: null,
    cancelText: null,
    text: '',
  });

  const showModal = (text, onApprove, onCancel = null, approveText=translate["approve"], cancelText=translate["cancel"]) => {
    setModalData({
      isModalVisible: true,
      onApprove,
      approveText,
      onCancel,
      cancelText,
      text,
    });
  };

  const hideModal = () => {
    setModalData((prevModalData) => ({
      ...prevModalData,
      isModalVisible: false,
    }));
  };

  return (
    <ModalContext.Provider value={{ showModal, hideModal }}>
      { children }
      <ConfirmModal
        isModalVisible={ modalData.isModalVisible }
        onApprove={ modalData.onApprove }
        apporveText={ modalData.approveText }
        closeModalFunc={ hideModal }
        onCancel={ modalData.onCancel }
        cancelText={ modalData.cancelText }
        text={ modalData.text }
      />
    </ModalContext.Provider>
  );
}
