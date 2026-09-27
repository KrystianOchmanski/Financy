import React from "react";
import { ConfirmAccountDeleteModalProps } from "../lib/definitions";

const ConfirmAccountDeleteModal: React.FC<ConfirmAccountDeleteModalProps> = ({
  isOpen,
  account,
  onClose,
  onConfirm,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-transparent backdrop-blur-xs">
      <div className="bg-white p-6 rounded shadow-lg">
        <p className="text-lg">
          Are you sure you want to delete account <b>{account?.name}</b>?
        </p>
        <div className="mt-4 flex justify-end gap-4">
          <button
            onClick={onClose}
            className="bg-gray-500 text-white px-4 py-2 rounded"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="bg-red-500 text-white px-4 py-2 rounded"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmAccountDeleteModal;
