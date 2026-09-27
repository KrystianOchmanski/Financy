import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AddAccountFormProps, addAccountSchema } from "../lib/definitions";
import { z } from "zod";
import { addAccount } from "./actions";
import { SubmitButton } from "../components/SubmitButton";
import toast from "react-hot-toast";

type FormData = z.infer<typeof addAccountSchema>;

export default function AddAccountModalForm({
  isOpen,
  onClose,
  onAccountAdded,
}: AddAccountFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: zodResolver(addAccountSchema),
  });

  const onSubmit = async (data: FormData) => {
    const result = await addAccount(data);

    if (!result.success) {
      return;
    }

    toast.success("Account added successfully!");
    reset();
    onAccountAdded();
    onClose();
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-transparent backdrop-blur-xs flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg w-full max-w-md shadow-lg">
        <h2 className="text-xl font-semibold mb-4">Add New Account</h2>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div>
            <label className="block mb-1 font-medium">Name</label>
            <input
              {...register("name")}
              name="name"
              className="w-full p-2 border rounded"
            />

            {errors.name && (
              <p className="text-red-500">{errors.name.message}</p>
            )}
          </div>

          <div className="mb-4">
            <label className="block mb-1 font-medium">Starting balance</label>
            <input
              type="number"
              step="0.01"
              {...register("startingBalance")}
              className="w-full p-2 border rounded"
            />
            {errors.startingBalance && (
              <p className="text-red-500">{errors.startingBalance.message}</p>
            )}
          </div>

          <div className="mt-6 flex justify-end space-x-2">
            <button
              type="button"
              onClick={handleClose}
              className="px-4 py-2 bg-gray-300 text-gray-800 rounded hover:bg-gray-400 transition"
            >
              Cancel
            </button>
            <SubmitButton
              pending={isSubmitting}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
            >
              Save
            </SubmitButton>
          </div>
        </form>
      </div>
    </div>
  );
}
