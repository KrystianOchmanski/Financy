import { useEffect, useState } from "react";
import {
  startOfMonth,
  endOfMonth,
  startOfYear,
  endOfYear,
  subMonths,
  format,
} from "date-fns";
import { Period, PeriodButton } from "../lib/definitions";

type Props = {
  onPeriodSelect: (period: Period) => void;
};

export default function PeriodButtons({ onPeriodSelect }: Props) {
  const now = new Date();

  const initialButtons: PeriodButton[] = [
    {
      title: "This month",
      period: {
        startDate: format(startOfMonth(now), "yyyy-MM-dd"),
        endDate: format(endOfMonth(now), "yyyy-MM-dd"),
      },
      selected: true,
    },
    {
      title: "Last month",
      period: {
        startDate: format(startOfMonth(subMonths(now, 1)), "yyyy-MM-dd"),
        endDate: format(endOfMonth(subMonths(now, 1)), "yyyy-MM-dd"),
      },
      selected: false,
    },
    {
      title: "This year",
      period: {
        startDate: format(startOfYear(now), "yyyy-MM-dd"),
        endDate: format(endOfYear(now), "yyyy-MM-dd"),
      },
      selected: false,
    },
    {
      title: "Last 12 months",
      period: {
        startDate: format(startOfMonth(subMonths(now, 12)), "yyyy-MM-dd"),
        endDate: format(endOfMonth(subMonths(now, 1)), "yyyy-MM-dd"),
      },
      selected: false,
    },
  ];

  const [buttons, setButtons] = useState<PeriodButton[]>(initialButtons);

  // <-- 1st render trigger
  useEffect(() => {
    const selectedButton = buttons.find((button) => button.selected);
    if (selectedButton) {
      onPeriodSelect(selectedButton.period);
    }
  }, []);

  const handleClick = (index: number) => {
    const updatedButtons = buttons.map((button, i) => ({
      ...button,
      selected: i === index,
    }));

    setButtons(updatedButtons);
    onPeriodSelect(updatedButtons[index].period);
  };

  return (
    <div className="inline-flex">
      {buttons.map((button, index) => (
        <button
          key={index}
          onClick={() => handleClick(index)}
          className={`px-4 py-2 font-semibold text-[14px] border border-gray-100 shadow-xs cursor-pointer
            ${
              button.selected
                ? "bg-blue-50 text-brand"
                : "bg-white text-gray-600 hover:bg-gray-50"
            } 
            ${index === 0 ? "rounded-l" : ""} 
            ${index === buttons.length - 1 ? "rounded-r" : ""} 
            `}
        >
          {button.title}
        </button>
      ))}
    </div>
  );
}
