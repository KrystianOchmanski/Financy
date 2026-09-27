"use client";

import useSWR from "swr";
import { useState } from "react";
import { Account, formatAmount } from "../lib/definitions";
import { fetcher } from "../lib/fetcher";
import AddAccountModalForm from "./AddAccountModalForm";
import { deleteAccount } from "./actions";
import toast from "react-hot-toast";
import { CiTrash } from "react-icons/ci";
import ConfirmAccountDeleteModal from "./ConfirmAccountDeleteModal";

export default function AccountPage() {
  const { data, error, isLoading, mutate } = useSWR<Account[]>(
    "/account",
    fetcher
  );
  const [showAddModal, setShowModal] = useState(false);
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [selectedAccount, setSelectedAccount] = useState<Account | null>(null);

  function handleDeleteClick(account: Account) {
    setSelectedAccount(account);
    setShowConfirmModal(true);
  }

  async function handleConfirmDelete() {
    if (selectedAccount !== null) {
      const result = await deleteAccount(selectedAccount.id);

      if (result.error) {
        toast.error(result.error);
      } else {
        toast.success("Account deleted successfully!");
        mutate();
      }
      setShowConfirmModal(false);
    }
  }

  if (isLoading) return <p className="text-gray-500">Loading...</p>;
  if (error) return <p className="text-red-500">Error loading accounts</p>;

  return (
    <>
      <AddAccountModalForm
        isOpen={showAddModal}
        onClose={() => setShowModal(false)}
        onAccountAdded={mutate}
      />
      <ConfirmAccountDeleteModal
        isOpen={showConfirmModal}
        account={selectedAccount}
        onClose={() => setShowConfirmModal(false)}
        onConfirm={handleConfirmDelete}
      />

      <button
        onClick={() => setShowModal(true)}
        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
      >
        Add account
      </button>

      <ul className="mt-4 space-y-2">
        {data?.map((account) => (
          <li
            key={account.id}
            className="flex justify-between border p-2 rounded shadow-sm"
          >
            <div>
              <strong>{account.name}</strong>: ${formatAmount(account.balance)}
            </div>
            <button
              className="hover:bg-gray-200 rounded cursor-pointer"
              onClick={() => handleDeleteClick(account)}
            >
              <CiTrash size={24} className="text-red-600" />
            </button>
          </li>
        ))}
      </ul>
    </>
  );
}
