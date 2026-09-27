import { ActionCardProps } from "../lib/definitions";

export default function ActionCard({ imgUrl, title, text }: ActionCardProps) {
  return (
    <div className="flex bg-white border border-gray-100 shadow-xs rounded-xl p-5 cursor-pointer hover:bg-gray-50">
      <img src={imgUrl} alt="Action Card Img" />
      <div className="ml-3">
        <p className="font-semibold text-gray-900 mb-0.5">{title}</p>
        <p className="text-gray-600 text-[14px]">{text}</p>
      </div>
    </div>
  );
}
